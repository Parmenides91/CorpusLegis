using CorpusLegis.Shared.Enums;

namespace CorpusLegis.API.Domain;


public class Suffragium
{
    public Guid Id { get; set; }

    public Guid RogatioId { get; set; }

    public Rogatio Rogatio { get; set; } = null!;


    public Guid CivisId { get; set; }

    public Civis Civis { get; set; } = null!;


    public SuffragiumValue Votum { get; set; }

    public DateTime CastAt { get; set; }

}
