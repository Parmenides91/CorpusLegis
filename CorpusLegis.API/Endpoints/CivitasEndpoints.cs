using CorpusLegis.API.Exceptions;
using CorpusLegis.API.Services;
using CorpusLegis.Shared.Dtos.Civitas;
using FluentValidation;
using MiniValidation;

namespace CorpusLegis.API.Endpoints;

public static class CivitasEndpoints
{

    public static void MapCivitasEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/civitas")
            .WithTags("Civitas")
            .RequireAuthorization()
            ;

        group.MapGet("/", GetAllCivitates);
        group.MapGet("/{id:guid}", GetCivitasById);
        group.MapPost("/", CreateCivitas);
        group.MapPut("/{id:guid}", UpdateCivitas);
        group.MapDelete("/{id:guid}", DeleteCivitas);


        group.MapGet("/civis/me", GetCivitatesForCurrentCivis);
        group.MapGet("/civis/{id:guid}", GetCivitatesForCivis);

        group.MapPost("/{civitasId:guid}/members/me", JoinCurrentCivisToCivitas);
        group.MapPost("/{civitasId:guid}/members/{civisId:guid}", JoinCivisToCivitas);

        group.MapDelete("/{civitasId:guid}/members/me", LeaveCurrentCivisFromCivitas);
        group.MapDelete("/{civitasId:guid}/members/{civisId:guid}", LeaveCivisFromCivitas);
    }



    private static async Task<IResult> GetAllCivitates(ICivitasService service)
    {
        var civitates = await service.GetAllAsync();

        return Results.Ok(civitates);
    }

    private static async Task<IResult> GetCivitasById(Guid id, ICivitasService service)
    {
        var civitas = await service.GetByIdAsync(id);

        return civitas is not null ? Results.Ok(civitas) : Results.NotFound();
    }

    private static async Task<IResult> CreateCivitas(CreateCivitasDto dto, IValidator<CreateCivitasDto> validator, ICivitasService service)
    {
        // Ya no uso MiniValidation.
        if (!MiniValidator.TryValidate(dto, out var errors))
        {
            return Results.ValidationProblem(errors);
        }
        // Uso FluentValidation.
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var _errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new BusinessRuleValidationException($"Errores de validación en la creación de la Civitas: {_errors}");
        }

        var result = await service.CreateAsync(dto);

        return Results.Created($"/civitas/{result.Id}", result); // si ha ido bien, será un código 201 + el objeto (DTO) creado.
    }

    private static async Task<IResult> UpdateCivitas(Guid id, UpdateCivitasDto dto, ICivitasService service)
    {
        if (!MiniValidator.TryValidate(dto, out var errors))
        {
            return Results.ValidationProblem(errors);
        }

        var updated = await service.UpdateAsync(id, dto);

        return updated is not null ? Results.Ok(updated) : Results.NotFound(); // si va bien, será un código 200 + el objeto (DTO) actualizado.
    }

    private static async Task<IResult> DeleteCivitas(Guid id, ICivitasService service)
    {
        var deleted = await service.DeleteAsync(id);

        return deleted ? Results.NoContent() : Results.NotFound();
    }



    private static async Task<IResult> GetCivitatesForCurrentCivis(ICivitasService service)
    {
        var civitates = await service.GetCivitatesForCurrentUserAsync();
        return Results.Ok(civitates);
    }

    private static async Task<IResult> GetCivitatesForCivis(Guid id, ICivitasService service)
    {
        var civitates = await service.GetCivitatesForUserAsync(id);
        return Results.Ok(civitates);
    }



    private static async Task JoinCurrentCivisToCivitas(Guid civitasId, ICivitasService service)
    {
        await service.AddCurrentCivisToCivitas(civitasId);
    }

    private static async Task JoinCivisToCivitas(Guid civitasId, Guid civisId, ICivitasService service)
    {
        await service.AddCivisToCivitas(civitasId, civisId);
    }



    private static async Task LeaveCurrentCivisFromCivitas(Guid civitasId, ICivitasService service)
    {
        await service.RemoveCurrentCivisFromCivitas(civitasId);
    }

    private static async Task LeaveCivisFromCivitas(Guid civitasId, Guid civisId, ICivitasService service)
    {
        await service.RemoveCivisFromCivitas(civitasId, civisId);
    }

}
