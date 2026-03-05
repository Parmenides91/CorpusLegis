using CorpusLegis.Shared.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CorpusLegis.API.Domain;

public class Rogatio
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public RogatioStatus Status { get; set; } = RogatioStatus.Inchoatus;

    public DateTime Deadline { get; set; }


    public decimal RequiredQuorum { get; set; } = 0.5m;

    public decimal RequiredMajority { get; set; } = 0.5m;


    public Guid CivisId { get; set; }
    public Civis Civis { get; set; } = null!;

    public Guid CivitasId { get; set; }
    public Civitas Civitas { get; set; } = null!;


    public ICollection<Sententia> Sententiae { get; set; } = new List<Sententia>();
    public ICollection<Suffragium> Suffragia { get; set; } = new List<Suffragium>();
}
