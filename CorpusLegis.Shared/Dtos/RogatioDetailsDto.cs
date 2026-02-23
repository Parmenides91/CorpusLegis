using CorpusLegis.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos;


public record RogatioDetailsDto(
    Guid Id,
    string Title,
    string Content,
    Guid AuthorId,
    DateTime CreatedAt,
    RogatioStatus Status
    );


