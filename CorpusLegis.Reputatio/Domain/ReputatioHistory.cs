using CorpusLegis.Contracts.ReputatioCalculus.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CorpusLegis.Reputatio.Domain;

public class ReputatioHistory
{

    [BsonRepresentation(BsonType.String)] // se fuerza que se guarde como string.
    public ReputatioAction Action { get; set; }


    public int Points { get; set; }


    public Guid ResourceId { get; set; }


    public DateTime Timestamp { get; set; }


}
