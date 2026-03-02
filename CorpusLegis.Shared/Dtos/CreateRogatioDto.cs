using CorpusLegis.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorpusLegis.Shared.Dtos;


// Las DTO de creación y actualización no pueden ser record, porque el model binder de ASP.NET Core no puede asignar valores a las propiedades de un record (que son init-only). Por eso, se usan clases normales con propiedades con getters y setters.
public class CreateRogatioDto
{
    //[Required(ErrorMessage = "Debes definir un título para crear una Rogatio.")]
    //[StringLength(100, ErrorMessage = "El título no debe exceder los 100 caracteres.")]
    public string Title { get; set; } = string.Empty;

    //[Required(ErrorMessage = "Las Rogationes deben tener un contenido en el momento de la creación.")]
    public string Content { get; set; } = string.Empty;

    public RogatioStatus Status { get; set; } = RogatioStatus.Inchoatus;

    public DateTime Deadline { get; set; }
}



/* No sé qué es todo esto, pero a ver si lo puedo poner en algún momento */
/*Al parecer, esto es FluentValidations, que se deberían poner en una carpeta Validators en el proyecto de Shared*/
//public class CreateRogatioDtoValidator : AbstractValidator<CreateRogatioDto>
//{
//    public CreateRogatioDtoValidator()
//    {
//        RuleFor(x => x.Title)
//            .NotEmpty().WithMessage("El título es obligatorio.")
//            .MaximumLength(200).WithMessage("El título no puede exceder los 200 caracteres.");
//        RuleFor(x => x.Content)
//            .NotEmpty().WithMessage("El contenido es obligatorio.");
//        RuleFor(x => x.AuthorId)
//            .NotEmpty().WithMessage("El ID del autor es obligatorio.");
//        RuleFor(x => x.CreatedAt)
//            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("La fecha de creación no puede ser futura.");
//        RuleFor(x => x.Status)
//            .NotEmpty().WithMessage("El estado es obligatorio.")
//            .Must(status => new[] { "Draft", "Voting", "Approved", "Rejected" }.Contains(status))
//            .WithMessage("El estado debe ser 'Draft', 'Voting', 'Approved' o 'Rejected'.");
//    }
//}