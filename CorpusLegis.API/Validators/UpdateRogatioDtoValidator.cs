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

        RuleFor(x => x.Deadline)
            .GreaterThan(DateTime.UtcNow.AddHours(24))
            .WithMessage("La fecha límite de votación debe ser al menos 24 horas en el futuro.");

        RuleFor(x => x.RequiredQuorum)
            .InclusiveBetween(0.01m, 1.00m)
            .WithMessage("El quórum necesario debe estar entre 0.01 y 1.00");

        RuleFor(x => x.RequiredMajority)
            .InclusiveBetween(0.01m, 1.00m)
            .WithMessage("La mayoría necesaria debe estar entre 0.01 y 1.00");
    }

}
