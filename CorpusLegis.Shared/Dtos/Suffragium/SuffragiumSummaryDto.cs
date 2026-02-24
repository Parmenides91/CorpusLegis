using CorpusLegis.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos.Suffragium;

public record SuffragiumSummaryDto(
    Guid Id,
    SuffragiumValue Votum
    );
