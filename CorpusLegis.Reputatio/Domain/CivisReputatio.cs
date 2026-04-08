using MongoDB.Bson.Serialization.Attributes;

namespace CorpusLegis.Reputatio.Domain;

public class CivisReputatio
{
    [BsonId]
    public Guid CivisId { get; set; }

    public int TotalScore { get; set; } = 0;

    public List<ReputatioHistory> History { get; set; } = new List<ReputatioHistory>();

    public DateTime LastUpdated { get; set; }
}
