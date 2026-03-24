using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace CorpusLegis.API.Infrastructure.Security;

public class KeycloakRolesClaimsTransformation : IClaimsTransformation
{

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Si el usuario no está autenticado, devolvemos el principal intacto.
        if (principal.Identity?.IsAuthenticated != true)
        {
            return Task.FromResult(principal);
        }

        // Si ya hemos mapeado los roles en esta petición, evitamos el procesamiento redundante.
        if (principal.HasClaim(c => c.Type == ClaimTypes.Role))
        {
            return Task.FromResult(principal);
        }

        var realmAccessClaim = principal.FindFirst("realm_access")?.Value;
        if (string.IsNullOrWhiteSpace(realmAccessClaim))
        {
            return Task.FromResult(principal);
        }

        // Clonamos el principal para no mutar el estado de seguridad original.
        var clonedPrincipal = principal.Clone();
        var additionalIdentity = new ClaimsIdentity();

        try
        {
            using var document = JsonDocument.Parse(realmAccessClaim);
            if (document.RootElement.TryGetProperty("roles", out var rolesElement))
            {
                foreach (var role in rolesElement.EnumerateArray())
                {
                    var roleValue = role.GetString();
                    if (!string.IsNullOrWhiteSpace(roleValue))
                    {
                        additionalIdentity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                    }
                }
            }
        }
        catch (JsonException)
        {
            // Un fallo en el parseo no debe detener la canalización, pero tampoco inyectar datos inválidos.
        }

        // Si encontramos roles, adjuntamos la nueva identidad al principal clonado.
        if (additionalIdentity.Claims.Any())
        {
            clonedPrincipal.AddIdentity(additionalIdentity);
        }

        return Task.FromResult(clonedPrincipal);
    }

}
