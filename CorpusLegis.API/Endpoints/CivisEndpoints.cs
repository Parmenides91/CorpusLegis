using CorpusLegis.API.Clients;
using CorpusLegis.API.Services;

namespace CorpusLegis.API.Endpoints;

public static class CivisEndpoints
{

    public static void MapCivisEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/civis")
            .WithTags("Civis")
            //.RequireAuthorization()
            ;

        group.MapGet("/{id:guid}/reputatio", GetCivisReputatio)
            .WithName("GetCivisReputatio")
            //.Produces<Shared.Dtos.Reputatio.ReputatioDto>(StatusCodes.Status200OK) // ¿esto lo pongo o lo quito? ¿qué hace?
            //.Produces(StatusCodes.Status404NotFound) // ¿esto lo pongo o lo quito? ¿qué hace?
            .WithSummary("Obtiene la reputación de un civis dado su ID")
            .WithDescription("Se consulta con la API de Reputatio para obtener la reputación del Civis indicado.")
            ;
    }


    private static async Task<IResult> GetCivisReputatio(Guid id, IReputatioClient reputatioClient, ICurrentUserService? currentUser, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("CivisEndpoints");

        //validar que el currentUser es el mismo que por el que se pide ver su reputación. ¿Quiero esto o quiero que todo el mundo pueda saber la reputación de todos los demás? ¿O quizá quiero que los Rector de las Civitas puedan saber la Reputatio de sus miembros?
        //if (currentUser.CivisId != id)
        //{
        //    logger.LogWarning("Acceso denegado: El usuario {UserId} intentó acceder a la reputación de {TargetId}",
        //    currentUser.CivisId, id);
        //    return Results.Forbid();
        //}

        var reputatio = await reputatioClient.GetReputatioAsync(id);

        return reputatio is not null
            ? Results.Ok(reputatio)
            : Results.NotFound();
    }


}
