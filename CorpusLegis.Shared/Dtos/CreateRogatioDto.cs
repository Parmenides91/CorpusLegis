using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorpusLegis.Shared.Dtos;

// Las DTO de creación y actualización no pueden ser record, porque el model binder de ASP.NET Core no puede asignar valores a las propiedades de un record (que son init-only). Por eso, se usan clases normales con propiedades con getters y setters.
//public record CreateRogatioDto(
//    Guid Id,
//    string Title,
//    string Content,
//    Guid AuthorId,
//    DateTime CreatedAt,
//    string Status
//    );

//public record CreateRogatioDto
//{
//    public Guid Id { get; set; }

//    public string Title { get; set; } = string.Empty;

//    public string Content { get; set; } = string.Empty;

//    public Guid AuthorId { get; set; } // el Civis

//    public DateTime CreatedAt { get; set; }

//    public string Status { get; set; } = "Draft"; // Draft, Voting, Approved (Lex), Rejected
//}


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


public class CreateRogatioDto
{

    // El Id no debería ir en el Create DTO, la bbdd o el backend es quien lo debe generar, no el formulario UI.

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(100, ErrorMessage = "El título no debe exceder los 100 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Toda Rogatio debe tener un contenido.")]
    public string Content { get; set; } = string.Empty;

    // El AuthorId lo inyectará el backend basándose en el usuario autentificado más adelante.

    public string Status { get; set; } = "Draft"; // Draft, Voting, Approved (Lex), Rejected
}