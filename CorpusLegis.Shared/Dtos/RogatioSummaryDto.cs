using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos;

public enum RogatioStatus
{
    Draft,
    Voting,
    Approved, // Lex
    Rejected
}

// DTO para listados (pocos campos).

public record RogatioSummaryDto(
    Guid Id,
    string Title,
    string Content,
    Guid AuthorId,
    DateTime CreatedAt,
    string Status
    );

