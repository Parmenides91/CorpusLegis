using CorpusLegis.Shared.Enums;

namespace CorpusLegis.API.Domain;

public class CivitasSodalis
{
    public Guid CivitasId { get; set; }
    public Civitas Civitas { get; set; } = null!;

    public Guid CivisId { get; set; }
    public Civis Civis { get; set; } = null!;

    public Munus Role { get; set; }
    public DateTime JoinedAt { get; set; }
}
