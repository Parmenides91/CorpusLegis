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
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                throw new InvalidOperationException("No hay contexto HTTP disponible.");
            }

            var headerValue = context.Request.Headers["X-Civis-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(headerValue) || !Guid.TryParse(headerValue, out var CivisId))
            {
                // Si no estuviéramos haciendo esta guarrada, esto sería un 401 Unauthorized.
                throw new UnauthorizedAccessException("Identidad de Civis no proporcionada o inválida en la cabecera.");
            }

            return CivisId;
        }
    }


}
