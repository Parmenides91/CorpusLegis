using CorpusLegis.Shared.Dtos.Civitas;
using FluentValidation;

namespace CorpusLegis.API.Validators;

public class CreateCivitasDtoValidator : AbstractValidator<CreateCivitasDto>
{

    public CreateCivitasDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Debes definir un título para crear una Civitas.")
            .MinimumLength(3).WithMessage("El título debe ser descriptivo (mínimo 3 caracteres).")
            .MaximumLength(100).WithMessage("El título no debe exceder los 100 caracteres.");

        RuleFor(x => x.Visibility)
            .IsInEnum().WithMessage("Las Civitates deben tener una visibilidad determinada en el momento de la creación.");

    }

}
