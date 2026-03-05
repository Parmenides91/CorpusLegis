using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos.Civitas;

public record CivitasSummaryDto(
    Guid Id,
    string Name,
    DateTime FoundedAt,
    bool IsMember = false
    );
