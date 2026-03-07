using CorpusLegis.Contracts.PdfGeneration;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.PdfWorker;

public class LexPromulgatedConsumer : IConsumer<LexPromulgatedIntegrationEvent>
{
    private readonly ILogger<LexPromulgatedConsumer> _logger;

    public LexPromulgatedConsumer(ILogger<LexPromulgatedConsumer> logger)
    {
        _logger = logger;
    }


    public async Task Consume(ConsumeContext<LexPromulgatedIntegrationEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Recibido el LexPromulgatedIntegrationEvent: {EventId}, Title: {Title}, PromulgatedAt: {PromulgatedAt}",
            message.EventId, message.Title, message.PromulgatedAt);

        // TODO: lógica de creación de PDF.
        await Task.Delay(1000);

        _logger.LogInformation("Se ha terminado el procesamiento del LexPromulgatedIntegrationEvent: {EventId}", message.EventId);
    }


}
