using CorpusLegis.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos.Invitatio;

public record InvitatioDetailsDto(
    Guid Id,
    Guid CivitasId,
    string CivitasName,
    string InviterName,
    string? InviteeName,
    string? InviteeEmail,
    DateTime IssuedAt,
    InvitationisStatus Status
    );
