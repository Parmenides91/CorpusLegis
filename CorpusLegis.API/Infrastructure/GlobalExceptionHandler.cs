using CorpusLegis.API.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CorpusLegis.API.Infrastructure;

public class GlobalExceptionHandler (ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment env) : IExceptionHandler
{

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Se ha producido una excepción no controlada: {Message}", exception.Message);

        // Mapeo de las excepciones.
        var (statusCode, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado."),
            UnauthorizedDomainException => (StatusCodes.Status403Forbidden, "Accedo denegado."),
            BusinessRuleValidationException or ArgumentException or InvalidOperationException => (StatusCodes.Status400BadRequest, "Error de validación de negocio."),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor.") // Fallback.
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Instance = httpContext.Request.Path,
            Detail = env.IsDevelopment() ? exception.Message : "Consulte los logs para más información.",
        };

        if (env.IsDevelopment())
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            problemDetails.Extensions["exceptionType"] = exception.GetType().FullName;
        }

        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;

    }

}
