using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos.Sententia;

public class CreateSententiaDto
{
    public Guid RogatioId { get; set; }
    public Guid? ParentId { get; set; } // nulo si es un comentario raíz.
    public string Content { get; set; } = string.Empty;
}
