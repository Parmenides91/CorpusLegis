using CorpusLegis.Shared.Dtos;
using FluentValidation;

namespace CorpusLegis.API.Validators;

public class UpdateRogatioDtoValidator : AbstractValidator<UpdateRogatioDto>
{

    public UpdateRogatioDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("No puedes dejar la Rogatio sin título.")
            .MinimumLength(3).WithMessage("El título debe ser descriptivo (mínimo 3 caracteres).")
            .MaximumLength(100).WithMessage("El título no debe exceder los 100 caracteres.");
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Las Rogationes deben tener un contenido en el momento de la actualización.");
    }

}
