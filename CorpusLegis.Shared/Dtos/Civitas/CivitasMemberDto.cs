using CorpusLegis.Shared.Enums;
using System;

namespace CorpusLegis.Shared.Dtos.Civitas;

public record CivitasMemberDto(
    Guid CivisId,
    string Name,
    Munus Role,
    DateTime JoinedAt
);
