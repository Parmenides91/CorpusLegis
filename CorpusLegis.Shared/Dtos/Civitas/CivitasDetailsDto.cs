using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos.Civitas;

public record CivitasDetailsDto(
    Guid Id,
    string Name,
    DateTime FoundedAt
    );
