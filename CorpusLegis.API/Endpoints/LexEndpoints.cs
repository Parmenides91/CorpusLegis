using CorpusLegis.API.Services;
using System.Runtime.CompilerServices;

namespace CorpusLegis.API.Endpoints;

public static class LexEndpoints
{
    public static void MapLexEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/lex").WithTags("Lex");

        group.MapGet("/", GetAllLeges);
        group.MapGet("/{id:guid}", GetLexById);
    }

    

    private static async Task<IResult> GetAllLeges(ILexService service)
    {
        var leges = await service.GetAllAsync();
        
        return Results.Ok(leges);
    }

    private static async Task<IResult> GetLexById(Guid id, ILexService service)
    {
        var lex = await service.GetByIdAsync(id);

        return lex is not null ? Results.Ok(lex) : Results.NotFound();
    }

}
