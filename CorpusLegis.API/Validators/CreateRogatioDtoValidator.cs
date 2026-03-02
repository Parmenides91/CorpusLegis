using CorpusLegis.Shared.Dtos;
using FluentValidation;

namespace CorpusLegis.API.Validators;

public class CreateRogatioDtoValidator : AbstractValidator<CreateRogatioDto>
{

    public CreateRogatioDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Debes definir un título para crear una Rogatio.")
            .MinimumLength(10).WithMessage("El título debe ser descriptivo (mínimo 10 caracteres).")
            .MaximumLength(100).WithMessage("El título no debe exceder los 100 caracteres.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Las Rogationes deben tener un contenido en el momento de la creación.");

        RuleFor(x => x.Deadline)
            .GreaterThan(DateTime.UtcNow.AddHours(24))
            .WithMessage("La fecha límite de votación debe ser al menos 24 horas en el futuro.");
    }

}
