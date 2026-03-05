using CorpusLegis.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorpusLegis.Shared.Dtos;


// Las DTO de creación y actualización no pueden ser record, porque el model binder de ASP.NET Core no puede asignar valores a las propiedades de un record (que son init-only). Por eso, se usan clases normales con propiedades con getters y setters.
public class CreateRogatioDto
{

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public RogatioStatus Status { get; set; } = RogatioStatus.Inchoatus;

    public DateTime Deadline { get; set; } = DateTime.UtcNow.AddHours(48);

    public Guid CivitasId { get; set; }


    public decimal RequiredQuorum { get; set; } = 0.5m;
    public decimal RequiredMajority { get; set; } = 0.5m;

}
