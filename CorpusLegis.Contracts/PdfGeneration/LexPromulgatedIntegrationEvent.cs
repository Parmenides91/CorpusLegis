using CorpusLegis.Contracts.Abstractions;

namespace CorpusLegis.Contracts.PdfGeneration;

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
