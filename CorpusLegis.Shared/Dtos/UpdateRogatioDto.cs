using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorpusLegis.Shared.Dtos;

//public record UpdateRogatioDto(
//    Guid Id,
//    string Title,
//    string Content,
//    Guid AuthorId,
//    DateTime CreatedAt,
//    string Status
//    );

//public record UpdateRogatioDto
//{
//    public Guid Id { get; set; }

//    public string Title { get; set; } = string.Empty;

//    public string Content { get; set; } = string.Empty;

//    public Guid AuthorId { get; set; } // el Civis

//    public DateTime CreatedAt { get; set; }

//    public string Status { get; set; } = "Draft"; // Draft, Voting, Approved (Lex), Rejected
//}


public class UpdateRogatioDto
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