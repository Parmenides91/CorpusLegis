using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos.Lex;

public record LexSummaryDto(
    Guid Id,
    string Title,
    Guid CivitasId,
    Guid OriginRogatioId,
    DateTime PromulgatedAt
    );
