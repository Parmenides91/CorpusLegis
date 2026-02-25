namespace CorpusLegis.API.Domain;

public class Sententia
{

    public Guid Id { get; set; }

    public Guid RogatioId { get; set; }

    public Rogatio Rogatio { get; set; } = null!;


    public Guid CivisId { get; set; }

    public Civis Civis { get; set; } = null!;


    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

}
