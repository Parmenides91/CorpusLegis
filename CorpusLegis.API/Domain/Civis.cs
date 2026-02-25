namespace CorpusLegis.API.Domain;

public class Civis
{

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;


    public ICollection<Civitas> Civitates { get; set; } = new List<Civitas>();
}
