using CorpusLegis.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos.Civitas;

public record CivitasDetailsDto(
    Guid Id,
    string Name,
    string Description,
    Visibilitas Visibility,
    DateTime FoundedAt
    );
