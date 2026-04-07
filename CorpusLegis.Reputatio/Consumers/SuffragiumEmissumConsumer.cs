using CorpusLegis.Contracts.ReputatioCalculus;
using CorpusLegis.Contracts.ReputatioCalculus.Enums;
using CorpusLegis.Reputatio.Configuration;
using CorpusLegis.Reputatio.Domain;
using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CorpusLegis.Reputatio.Consumers;

public class SuffragiumEmissumConsumer(IMongoClient mongoClient, IOptions<ReputatioRulesOptions> rulesOptions, ILogger<SuffragiumEmissumConsumer> logger)
    : IConsumer<SuffragiumEmissumIntegrationEvent>
{

    public async Task Consume(ConsumeContext<SuffragiumEmissumIntegrationEvent> context)
    {
        var msg = context.Message;
        logger.LogInformation("Procesando SuffragiumEmissumIntegrationEvent para Civis {CivisId}", msg.CivisId);

        var db = mongoClient.GetDatabase("mongo-db-corpuslegis");
        var collection = db.GetCollection<CivisReputatio>("civis_reputatio");

        var newHistoryEntry = new ReputatioHistory
        {
            Action =ReputatioAction.SuffragiumEmissum,
            Points = rulesOptions.Value.SuffragiumEmissum,
            ResourceId = msg.RogatioId,
            Timestamp = msg.Timestamp
        };

        var updateDefinition = Builders<CivisReputatio>.Update
            .Inc(cr => cr.TotalScore, newHistoryEntry.Points)
            .Push(cr => cr.History, newHistoryEntry)
            .Set(cr => cr.LastUpdated, DateTime.UtcNow);

        await collection.UpdateOneAsync(
            cr => cr.CivisId == msg.CivisId,
            updateDefinition,
            new UpdateOptions { IsUpsert = true }
        );
    }

}
