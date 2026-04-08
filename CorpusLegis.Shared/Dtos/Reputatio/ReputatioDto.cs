using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos.Reputatio;

public record ReputatioDto(
    Guid CivisId,
    int TotalScore,
    DateTime LastUpdated
    );
