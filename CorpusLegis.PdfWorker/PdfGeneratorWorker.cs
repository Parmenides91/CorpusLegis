namespace CorpusLegis.PdfWorker;

public class PdfGeneratorWorker(ILogger<PdfGeneratorWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                //logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                logger.LogInformation("PdfGeneratorWorker: Buscando Rogationes pendientes de procesar | [{time}]", DateTimeOffset.Now);
            }
            await Task.Delay(10000, stoppingToken); // 10 segundos de espera
        }
    }
}
