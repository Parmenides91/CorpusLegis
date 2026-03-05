using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos.Lex;

public record LexDetailsDto(
    Guid Id,
    string Title,
    string Content,
    Guid CivitasId,
    Guid OriginRogatioId,
    DateTime PromulgatedA
    );
