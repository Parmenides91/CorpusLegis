using CorpusLegis.Contracts.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Contracts.InvitatioProcess;

//public interface IIntegrationEvent
//{
//    // Marker interface for integration events
//    Guid EventId { get; }
//    DateTimeOffset OccurredAt { get; }
//    string? CorrelationId { get; }
//    int EventVersion { get; }
//}


public class InvitatioAcceptedIntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public string? CorrelationId { get; init; }
    public int EventVersion { get; init; } = 1;



    public Guid InvitatioId { get; init; }

    public Guid CivitasId { get; init; }

    public Guid CivisId { get; init; }

    public DateTime ProcessedAt { get; init; }

}
