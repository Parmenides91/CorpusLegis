using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos.Invitatio;

public record CreateInvitatioDto(
    Guid CivitasId,
    string InviteeEmail
    );
