using CorpusLegis.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorpusLegis.Shared.Dtos;


public class UpdateRogatioDto
{
    // El Id no debería ir en el Create DTO, la bbdd o el backend es quien lo debe generar, no el formulario UI.

    [Required(ErrorMessage = "No puedes dejar la Rogatio sin título.")]
    [StringLength(100, ErrorMessage = "El título no debe exceder los 100 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "No puedes dejar el cuerpo de la Rogatio sin contenido.")]
    public string Content { get; set; } = string.Empty;

    // El AuthorId lo inyectará el backend basándose en el usuario autentificado más adelante.

    public RogatioStatus Status { get; set; } = RogatioStatus.Inchoatus;
}