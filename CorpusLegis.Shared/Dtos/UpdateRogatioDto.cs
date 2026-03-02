using CorpusLegis.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorpusLegis.Shared.Dtos;


public class UpdateRogatioDto
{
    //[Required(ErrorMessage = "No puedes dejar la Rogatio sin título.")]
    //[StringLength(100, ErrorMessage = "El título no debe exceder los 100 caracteres.")]
    public string Title { get; set; } = string.Empty;

    //[Required(ErrorMessage = "No puedes dejar el cuerpo de la Rogatio sin contenido.")]
    public string Content { get; set; } = string.Empty;

    public RogatioStatus Status { get; set; } = RogatioStatus.Inchoatus;
}