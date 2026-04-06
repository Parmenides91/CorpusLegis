using CorpusLegis.Shared.Dtos.Sententia;
using FluentValidation;

namespace CorpusLegis.API.Validators;

public class CreateSententiaDtoValidator : AbstractValidator<CreateSententiaDto>
{
    public CreateSententiaDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("El contenido de la Sententia no puede estar vacío.")
            .MinimumLength(2).WithMessage("El contenido de la Sententia debe ser al menos de 2 caracteres.")
            .MaximumLength(150).WithMessage("El contenido de la Sententia no debe superar los 150 caracteres.");
    }
}
