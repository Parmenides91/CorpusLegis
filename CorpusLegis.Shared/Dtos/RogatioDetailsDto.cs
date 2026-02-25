using CorpusLegis.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos;


public record RogatioDetailsDto(
    Guid Id,
    string Title,
    string Content,
    Guid CivisId,
    string CivisName,
    Guid CivitasId,
    string CivitasName,
    DateTime CreatedAt,
    RogatioStatus Status,
    int ProVotes,
    int ContraVotes,
    int AbstentioVotes,
    bool HasVoted
    );


