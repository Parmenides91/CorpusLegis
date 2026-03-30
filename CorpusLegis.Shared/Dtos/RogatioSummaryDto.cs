using CorpusLegis.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos;

public record RogatioSummaryDto(
    Guid Id,
    string Title,
    string Content,
    Guid CivisId,
    string CivitasName,
    DateTime CreatedAt,
    RogatioStatus Status,
    bool CanEdit,
    bool CanDelete
    );

