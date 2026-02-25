using CorpusLegis.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos;

public record RogatioSummaryDto(
    Guid Id,
    string Title,
    string Content,
    //Guid AuthorId,
    Guid CivisId,
    DateTime CreatedAt,
    RogatioStatus Status
    );

