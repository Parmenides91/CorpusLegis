using CorpusLegis.API.Domain;
using CorpusLegis.API.Services;
using CorpusLegis.Shared.Dtos;
using MiniValidation;

namespace CorpusLegis.API.Endpoints;

public static class RogatioEndpoints
{

    public static void MapRogatioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/rogatio").WithTags("Rogatio");

        // Se enlazan las rutas a métodos locales
        group.MapGet("/", GetAllRogationes);
        group.MapGet("/{id:guid}", GetRogatioById);
        group.MapPost("/", CreateRogatio);
        group.MapPut("/{id:guid}", UpdateRogatio);
        group.MapDelete("/{id:guid}", DeleteRogatio);

        group.MapPut("/{id:guid}/status", TransicionarRogatio);
        group.MapPut("/{id:guid}/evaluation", EvaluarRogatio);
        group.MapPost("/evaluate-pending", EvaluatePendingRogationes);

    }

    

    private static async Task<IResult> GetAllRogationes(IRogatioService service)
    {
        var rogationes = await service.GetAllAsync();

        return Results.Ok(rogationes);
    }

    private static async Task<IResult> GetRogatioById(Guid id, IRogatioService service)
    {
        var rogatio = await service.GetByIdAsync(id);

        return rogatio is not null ? Results.Ok(rogatio) : Results.NotFound();
    }

    private static async Task<IResult> CreateRogatio(CreateRogatioDto dto, IRogatioService service)
    {
        if (!MiniValidator.TryValidate(dto, out var errors))
        {
            return Results.ValidationProblem(errors);
        }

        var result = await service.CreateAsync(dto);

        return Results.Created($"/rogatio/{result.Id}", result); // si ha ido bien, será un código 201 + el objeto (DTO) creado.
    }

    private static async Task<IResult> UpdateRogatio(Guid id, UpdateRogatioDto dto, IRogatioService service)
    {
        if (!MiniValidator.TryValidate(dto, out var errors))
        {
            return Results.ValidationProblem(errors);
        }

        var updated = await service.UpdateAsync(id, dto);

        return updated is not null ? Results.Ok(updated) : Results.NotFound(); // si va bien, será un código 200 + el objeto (DTO) actualizado.
    }

    private static async Task<IResult> DeleteRogatio(Guid id, IRogatioService service)
    {
        var deleted = await service.DeleteAsync(id);

        return deleted ? Results.NoContent() : Results.NotFound();
    }

    
    private static async Task<IResult> TransicionarRogatio(Guid id, WorkflowRogatioDto dto, IRogatioService service)
    {
        if (!MiniValidator.TryValidate(dto, out var errors))
        {
            return Results.ValidationProblem(errors);
        }

        try
        {
            var result = await service.ChangeStatusAsync(id, dto);
            return result != null ? Results.Ok(result) : Results.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> EvaluarRogatio(Guid id, WorkflowRogatioDto dto, IRogatioService service)
    {
        try
        {
            var result = await service.EvalueAsync(id, dto);
            return result != null ? Results.Ok(result) : Results.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }


    private static async Task<IResult> EvaluatePendingRogationes(IRogatioService service)
    {
        try
        {
            int count = await service.EvaluatePendingAsync();
            return Results.Ok(new { evaluatedCount = count });
        }
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message, title: "Error during batch evaluation");
        }
    }



}
