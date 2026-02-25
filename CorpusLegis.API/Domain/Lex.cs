namespace CorpusLegis.API.Domain;

public class Lex
{

    public Guid Id { get; set; }

    public Guid CivitasId { get; set; }

    public Civitas Civitas { get; set; } = null!;



    // Rogatio que le dió origen
    public Guid OriginRogatioId { get; set; }
    public Rogatio OriginRogatio { get; set; } = null!;



    public string Title { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
    
    public DateTime PromulgatedAt { get; set; }
    


    // Para el historial de enmiendas. Si no es null, ésta ley está derogada por la nueva Lex
    public Guid? DerogatedByLexId { get; set; }

    public Lex? DerogatedByLex { get; set; }
}
