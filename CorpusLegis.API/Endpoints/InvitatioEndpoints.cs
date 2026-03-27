using CorpusLegis.API.Services;
using CorpusLegis.Shared.Dtos.Invitatio;
using FluentValidation;

namespace CorpusLegis.API.Endpoints;

public static class InvitatioEndpoints
{

    public static void MapInvitatioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/invitatio")
            .WithTags("Invitatio")
            .RequireAuthorization()
            ;

        group.MapPost("/", SendInvitatio);
        group.MapPost("/{id:guid}/revoke", RevokeInvitatio);
        group.MapGet("/civitas/{civitasId:guid}/pending", GetPendingForCivitas);

        group.MapGet("/civis/{civisId:guid}/pending", GetPendingForCivis);
        group.MapGet("/me/pending", GetPendingForCurrentCivis);
        group.MapPost("/{id:guid}/accept", AcceptInvitatio);
        group.MapPost("/{id:guid}/reject", RejectInvitatio);
    }


    #region emisores
    private static async Task<IResult> SendInvitatio(CreateInvitatioDto dto, IValidator<CreateInvitatioDto> validator, IInvitatioService service)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await service.SendAsync(dto);

        //return Results.Created($"/invitatio/{result.id}", result); // esto significaría que tengo una página para ver la Invitatio. Así que o la creo (método + endpoint + UI) o devuelvo el listado de Invitaciones del Civis o de la Civitas en la que esté.
        return Results.Ok(result);
    }

    private static async Task<IResult> RevokeInvitatio(Guid id, IInvitatioService service)
    {
        var revoked = await service.RevokeAsync(id);

        return revoked ? Results.Ok() : Results.BadRequest("No se ha podido revocar la Invitatio. Verifica los permisos o el estado de la misma.");
    }

    private static async Task<IResult> GetPendingForCivitas(Guid civitasId, IInvitatioService service)
    {
        var invitationes = await service.GetPendingForCivitasAsync(civitasId);

        return Results.Ok(invitationes);
    }
    #endregion


    #region receptores
    private static async Task<IResult> GetPendingForCivis(Guid civisId, IInvitatioService service)
    {
        var invitationes = await service.GetPendingInvitationesForCivisAsync(civisId);

        return Results.Ok(invitationes);
    }

    private static async Task<IResult> GetPendingForCurrentCivis(IInvitatioService service)
    {
        var invitationes = await service.GetPendingInvitationesForCurrentCivisAsync();

        return Results.Ok(invitationes);
    }

    private static async Task<IResult> AcceptInvitatio(Guid id, IInvitatioService service)
    {
        var accepted = await service.AcceptAsync(id);

        return accepted ? Results.Ok() : Results.BadRequest("No se ha podido aceptar la Invitatio. Puede haber expirado o ser inválida.");
    }

    private static async Task<IResult> RejectInvitatio(Guid id, IInvitatioService service)
    {
        var rejected = await service.RejectAsync(id);

        return rejected ? Results.Ok() : Results.BadRequest("No se ha podido rechazar la Invitatio.");
    }
    #endregion

}
