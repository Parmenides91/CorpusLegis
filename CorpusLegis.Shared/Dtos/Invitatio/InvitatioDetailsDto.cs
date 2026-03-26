using CorpusLegis.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos.Invitatio;

public record InvitatioDetailsDto(
    Guid id,
    Guid CivitasId,
    string CivitasName,
    string InviterName,
    DateTime SentAt,
    InvitationisStatus Status
    );
