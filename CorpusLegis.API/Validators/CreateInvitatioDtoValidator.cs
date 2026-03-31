using CorpusLegis.Shared.Dtos.Invitatio;
using FluentValidation;

namespace CorpusLegis.API.Validators;

public class CreateInvitatioDtoValidator : AbstractValidator<CreateInvitatioDto>
{

    public CreateInvitatioDtoValidator()
    {
        RuleFor(x => x.CivitasId)
            .NotEmpty().WithMessage("La Invitatio debe estar ligada a una Civitas.");

        RuleFor(x => x.InviteeEmail)
            .NotEmpty().WithMessage("Debes proporcionar el correo electrónico del invitado.")
            .EmailAddress().WithMessage("El formato del correo electrónico proporcionado no es válido.");

    }

}
