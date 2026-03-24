//using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            // Esto fue la primera implementación para tener un sistema de usuarios mockeado.
            //var context = _httpContextAccessor.HttpContext;
            //if (context == null)
            //{
            //    throw new InvalidOperationException("No hay contexto HTTP disponible.");
            //}
            //var headerValue = context.Request.Headers["X-Civis-Id"].FirstOrDefault();
            //if (string.IsNullOrEmpty(headerValue) || !Guid.TryParse(headerValue, out var CivisId))
            //{
            //    // Si no estuviéramos haciendo esta guarrada, esto sería un 401 Unauthorized.
            //    throw new UnauthorizedAccessException("Identidad de Civis no proporcionada o inválida en la cabecera.");
            //}
            //return CivisId;

            // Esto ha sido un intento de tener usuarios reales.
            //var user = _httpContextAccessor.HttpContext?.User;
            //if (user == null || user.Identity?.IsAuthenticated != true)
            //{
            //    throw new UnauthorizedAccessException("Usuario no autentificado. No se ha encontrado un contexto HTTP válido o el usuario no ha iniciado sesión");
            //}
            //var userClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userClaim) || !Guid.TryParse(userClaim, out var civisId))
            //{
            //    throw new UnauthorizedAccessException("El token JWT no contiene un identificador de usuario válido ('sub').");
            //}
            //return civisId;

            // Tercer intento de tener usuarios.
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
