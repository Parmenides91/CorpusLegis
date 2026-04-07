using CorpusLegis.Contracts.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Contracts.ReputatioCalculus;

public record RogatioCreatedIntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public string? CorrelationId { get; init; }
    public int EventVersion { get; init; } = 1;




    public Guid CivisId { get; init; }
    public Guid RogatioId { get; init; }
    public DateTime Timestamp { get; init; }
}
