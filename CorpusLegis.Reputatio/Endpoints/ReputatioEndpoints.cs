using CorpusLegis.Reputatio.Domain;
using CorpusLegis.Reputatio.Services;

namespace CorpusLegis.Reputatio.Endpoints;

public static class ReputatioEndpoints
{

    public static void MapReputatioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/reputatio")
            .WithTags("Reputatio")
            ;

        group.MapGet("/{id:guid}", GetCivisReputatio)
            .WithName("GetCivisReputatio")
            .Produces<CivisReputatio>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Obtener reputatio de un civis por su ID.") // TODO: usar WithSummary() para todos los endpoins.
            .WithDescription("Devuelve la puntuación total y el historial de acciones que han afectado la reputatio.") // TODO: usar WithDescription() para todos los endpoins.
            ;

    }

    private static async Task<IResult> GetCivisReputatio(Guid id, IReputatioService service, CancellationToken ct)
    {
        var reputatio = await service.GetByCivisIdAsync(id, ct);
        return reputatio is not null
            ? Results.Ok(reputatio)
            : Results.NotFound();
    }

}
