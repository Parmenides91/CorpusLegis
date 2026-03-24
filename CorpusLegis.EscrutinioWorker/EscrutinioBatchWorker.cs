namespace CorpusLegis.EscrutinioWorker;

public class EscrutinioBatchWorker : BackgroundService
{

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EscrutinioBatchWorker> _logger;

    public EscrutinioBatchWorker(IHttpClientFactory httpClientFactory, ILogger<EscrutinioBatchWorker> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //while (!stoppingToken.IsCancellationRequested)
        //{
        //    if (logger.IsEnabled(LogLevel.Information))
        //    {
        //        logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //    }
        //    await Task.Delay(1000, stoppingToken);
        //}

        _logger.LogInformation("Worker de Escrutinio iniciado a las: {time}", DateTimeOffset.Now);

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30)); // Ejecutar cada 30 segundos

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ProcessPendingEscrutiniosAsync(stoppingToken);
        }

    }


    private async Task ProcessPendingEscrutiniosAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Ejecutando lote de evaluación de Rogationes pendientes...");

            //var response = await _httpClient.PostAsync("/rogatio/evaluate-pending", null, stoppingToken);

            var client = _httpClientFactory.CreateClient("CorpuesLegisApiClient");
            //client.DefaultRequestHeaders.Add("X-Civis-Id", "0f8fad5b-d9cb-469f-a165-70867728950e"); // TODO: corregir cuando tengamos autenticación real --> esto se resuelve mediante el patrón Machine-to-Machine (M2M) utilizando el flujo Client Credentials de OAuth2. El Worker solicitará un token JWT al servidor de identidad (Keycloak) identificándose como un servicio, no como un usuario. La API tendrá endpoints divididos
            var response = await client.PostAsync("/rogatio/evaluate-pending", null, stoppingToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("Lote de evaluación ejecutado correctamente.");
            }
            else
            {
                _logger.LogWarning("La API ha devuelto un código no exitoso al evaluar Rogationes: {StatusCode}", response.StatusCode);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error al comunicarse con la API para evaluar Rogationes pendientes.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Error crítico al ejecutar el lote de evaluación de Rogationes pendientes. Detalles del error: {Message}", ex.Message);
        }

    }


}
