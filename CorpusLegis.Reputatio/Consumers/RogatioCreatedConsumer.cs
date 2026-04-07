
using CorpusLegis.Contracts.ReputatioCalculus;
using CorpusLegis.Contracts.ReputatioCalculus.Enums;
using CorpusLegis.Reputatio.Domain;
using MassTransit;
using MongoDB.Driver;
using OpenTelemetry.Metrics;

namespace CorpusLegis.Reputatio.Consumers;

public class RogatioCreatedConsumer (IMongoClient mongoClient, ILogger<RogatioCreatedConsumer> logger)
    : IConsumer<RogatioCreatedIntegrationEvent>
{

    public async Task Consume(ConsumeContext<RogatioCreatedIntegrationEvent> context)
    {
        var msg = context.Message;
        logger.LogInformation("Procesando evento RogatioCreatedIntegrationEvent {EventId} de RogatioCreated para Civis {CivisId} de Rogatio: {RogatioId}",
            msg.EventId, msg.CivisId, msg.RogatioId);

        var db = mongoClient.GetDatabase("mongo-db-corpuslegis");
        var collection = db.GetCollection<CivisReputatio>("civis_reputatio"); // ¿colecciones en MongoDB? --> snake_case.

        var newHistoryEntry = new ReputatioHistory
        {
            //Action = "RogatioCreated",
            Action = ReputatioAction.RogatioCreated,
            Points = 10, // TODO: esta definición de puntos ¿debe ir en el appsettings.json?
            //RogatioId = msg.RogatioId,
            Timestamp = msg.Timestamp,
            //EventType = nameof(RogatioCreatedIntegrationEvent)
        };

        var updateDefinition = Builders<CivisReputatio>.Update
            .Inc(cr => cr.TotalScore, newHistoryEntry.Points)
            .Push(cr => cr.History, newHistoryEntry)
            .Set(cr => cr.LastUpdated, DateTime.UtcNow);

        var options = new UpdateOptions { IsUpsert = true }; //¿qué hace ésto?

        await collection.UpdateOneAsync(
            cr => cr.CivisId == msg.CivisId,
            updateDefinition,
            options);

        logger.LogInformation("Evento RogatioCreatedIntegrationEvent procesado para Civis {CivisId} de Rogatio: {RogatioId}. Reputación actualizada.",
            msg.CivisId, msg.RogatioId);
    }

}
