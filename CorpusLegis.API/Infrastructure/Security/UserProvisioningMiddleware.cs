namespace CorpusLegis.API.Infrastructure.Security;

public class UserProvisioningMiddleware
{
    private readonly RequestDelegate _next;

    public UserProvisioningMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    // El servicio se inyecta en el método InvokeAsync porque es Scoped (depende del DbContext), 
    // mientras que el Middleware es un Singleton instanciado al arrancar.
    public async Task InvokeAsync(HttpContext context, IUserProvisioningService provisioningService, ILogger<UserProvisioningMiddleware> logger)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var isMachine = context.User.HasClaim(c => c.Type == "clientId" || c.Type == "client_id");

            if (!isMachine)
            {
                try
                {
                    await provisioningService.ProvisionUserAsync(context.User, context.RequestAborted);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error durante el aprovisionamiento del Civis.");
                    // No se detiene la solicitud, se continúa aunque falle el aprovisionamiento.
                }
            }
        }

        await _next(context);
    }

}
