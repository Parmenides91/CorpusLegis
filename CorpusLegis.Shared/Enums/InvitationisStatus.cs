using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Enums;

public enum InvitationisStatus
{
    Pendens,    // Pendiente de respuesta.
    Accepta,    // Aceptada.
    Repudiata,  // Rechazada por el invitado.
    Revocata,   // Revocada por el emisor antes de que el invitado respondiera.
    Expirata    // Expirada automáticamente.
}
