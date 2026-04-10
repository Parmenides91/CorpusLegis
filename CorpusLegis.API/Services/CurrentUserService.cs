//using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace CorpusLegis.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid CivisId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || user.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException("Usuario no autentificado. No se ha encontrado un contexto HTTP válido o el usuario no ha iniciado sesión. Usuario no autenticado. No se encontró un contexto HTTP válido o el usuario no ha iniciado sesión.");
            }

            // se busca explícitamente el claim estándar 'sub' (Subject)
            var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? user.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var civisId))
            {
                throw new UnauthorizedAccessException($"El token JWT no contiene un identificador de usuario válido en el claim 'sub'. Valor recibido: {userIdClaim ?? "NULL"}");
            }

            return civisId;
        }

    }


}
