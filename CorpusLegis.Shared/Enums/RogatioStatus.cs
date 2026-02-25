using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Enums;

public enum RogatioStatus
{
    Inchoatus,          // Borrador, sólo editable por el autor.
    Proposita,          // Presentada, se abre el debate (comentarios).
    InSuffragium,       // En votación, se bloquean comentario y se abren votos.
    Approbata,          // Aprobada, pasa a ser Lex.
    Reprobata           // Rechazada, fin del ciclo.
}
