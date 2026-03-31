using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Contracts.Abstractions;

public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredAt { get; }
    string? CorrelationId { get; }
    int EventVersion { get; }
}
