using CorpusLegis.API.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CorpusLegis.API.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Se ha producido una excepción no controlada: {Message}", exception.Message);

        // Mapeo de las excepciones.
        var (statusCode, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado."),
            UnauthorizedDomainException => (StatusCodes.Status403Forbidden, "Accedo denegado."),
            BusinessRuleValidationException => (StatusCodes.Status400BadRequest, "Error de validación de negocio."),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor.") // Fallback.
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            // TODO: Filtrar si estamos en PRODUCCIÓN o en cualquier otro entorno.
            //problemDetails.Detail = "Se ha producido un error inesperado. Contacte con alguien que sepa lo que hace.";
        }

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;

    }

}
