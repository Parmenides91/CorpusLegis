namespace CorpusLegis.API.Domain;

public class Civitas
{

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime FoundedAt { get; set; }

    
    public ICollection<Civis> Cives { get; set; } = new List<Civis>();
    public ICollection<Rogatio> Rogationes { get; set; } = new List<Rogatio>();
    public ICollection<Lex> Leges { get; set; } = new List<Lex>();
}
