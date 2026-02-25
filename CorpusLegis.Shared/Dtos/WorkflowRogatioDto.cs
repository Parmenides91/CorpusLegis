using CorpusLegis.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorpusLegis.Shared.Dtos;

public class WorkflowRogatioDto
{
    public Guid Id { get; set; }

    public RogatioStatus Status { get; set; }
}
