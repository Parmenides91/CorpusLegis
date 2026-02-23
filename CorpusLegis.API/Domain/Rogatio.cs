using CorpusLegis.Shared.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CorpusLegis.API.Domain;

public class Rogatio
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

//    public Guid AuthorId { get; set; } // el Civis

    public DateTime CreatedAt { get; set; }

    //public string Status { get; set; } = "Draft"; // Draft, Voting, Approved (Lex), Rejected

    public RogatioStatus Status { get; set; } = RogatioStatus.Inchoatus;


    public Guid CivisId { get; set; }

    public Guid CivitasId { get; set; }
}
