using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;

namespace CorpusLegis.API.Infrastructure.Security;

public sealed class UserProvisioningService : IUserProvisioningService
{
    
    private readonly CorpusLegisContext _dbContext;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<UserProvisioningService> _logger;

    public UserProvisioningService(CorpusLegisContext dbContext, IDistributedCache distributedCache, ILogger<UserProvisioningService> logger)
    {
        _dbContext = dbContext;
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task ProvisionUserAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        // Se extrae el claim 'sub' (subject). En JWT estçandar y Keycloak es el NameIdentifier.
        var subClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(subClaim) || !Guid.TryParse(subClaim, out Guid userId))
        {
            _logger.LogWarning("Intento de aprovisionamiento fallido: Claim 'sub' ausente o no es un GUID válido.");
            return;
        }

        // Verificación en caché primero.
        string distributedCacheKey = $"corpuslegis:UserProvisioning:{userId}";
        var cachedValue = await _distributedCache.GetStringAsync(distributedCacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedValue))
        {
            return; // el usuario ya ha sido aprovisionado recientemente.
        }

        // Se extraen datos adicionales del Token (ajustar según los claims exactos que emita mi Keycloak).
        var email = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("email")?.Value ?? string.Empty;
        var name = user.FindFirst("preferred_username")?.Value ?? user.FindFirst("name")?.Value ?? "Desconocido";

        // Verificación e inserción/actualización en la base de datos.
        var civis = await _dbContext.Cives.FirstOrDefaultAsync(c => c.Id == userId, cancellationToken);

        if (civis == null)
        {
            civis = new Civis
            {
                Id = userId,
                Email = email,
                Name = name
            };

            _dbContext.Cives.Add(civis);
            _logger.LogInformation("Aprovisionado nuevo Civis: {UserId} - {Name} - {Email}", userId, name, email);
        }
        else
        {
            bool updated = false;

            if (civis.Email != email)
            {
                civis.Email = email;
                updated = true;
            }

            if (civis.Name != name)
            {
                civis.Name = name;
                updated = true;
            }

            if (updated)
            {
                _logger.LogInformation("Actualizando datos del Civis existente: {UserId} - {Name} - {Email}", userId, name, email);
            }
        }

        try
        {
            if (_dbContext.ChangeTracker.HasChanges())
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            // Se añade a la caché para evitar verificaciones repetidas en un corto período y reducir la carga en la base de datos.
            var distributedCacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            await _distributedCache.SetStringAsync(distributedCacheKey, "1", distributedCacheOptions, cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            // Manejo de condición de carrera: Si dos peticiones simultáneas del mismo usuario nuevo
            // intentan insertarlo, la restricción de clave primaria fallará en una de ellas.
            _logger.LogWarning(ex, "Condición de carrera al aprovisionar usuario {UserId}. Es seguro ignorarlo.", userId);
        }

    }

}
