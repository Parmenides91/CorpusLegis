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

public record RogatioDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public Guid AuthorId { get; set; } // el Civis

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = "Draft"; // Draft, Voting, Approved (Lex), Rejected

    //public RogatioStatus Status { get; set; } = RogatioStatus.Draft;
}
