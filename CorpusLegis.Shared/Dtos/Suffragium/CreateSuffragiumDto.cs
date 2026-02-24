using CorpusLegis.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos.Suffragium;

public class CreateSuffragiumDto
{
    public Guid RogatioId { get; set; }
    public Guid CivisId { get; set; }
    public SuffragiumValue Votum { get; set; }

}
