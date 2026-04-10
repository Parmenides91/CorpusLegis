using CorpusLegis.Shared.Enums;


namespace CorpusLegis.Shared.Dtos;


public class UpdateRogatioDto
{

    public string Title { get; set; } = string.Empty;


    public string Content { get; set; } = string.Empty;

    public RogatioStatus Status { get; set; } = RogatioStatus.Inchoatus;

    public DateTime Deadline { get; set; }

    public decimal RequiredQuorum { get; set; }
    public decimal RequiredMajority { get; set; }

}
