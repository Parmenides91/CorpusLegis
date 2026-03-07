namespace CorpusLegis.Contracts.PdfGeneration;


public interface IIntegrationEvent
{
    // Marker interface for integration events
    Guid EventId { get; }
    DateTimeOffset OccurredAt { get; }
    string? CorrelationId { get; }
    int EventVersion { get; }
}

public record LexPromulgatedIntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public string? CorrelationId { get; init; }
    public int EventVersion { get; init; } = 1;




    public Guid LexId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public DateTime PromulgatedAt { get; init; }
}
