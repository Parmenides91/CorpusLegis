namespace CorpusLegis.API.Domain;

public class Sententia
{

    public Guid Id { get; set; }

    public Guid RogatioId { get; set; }

    public Rogatio Rogatio { get; set; } = null!;

    public Guid CivisId { get; set; }

    public Civis Civis { get; set; } = null!;


    // Jerarquía.
    public Guid? ParentId { get; set; }
    public Sententia? Parent { get; set; }
    public ICollection<Sententia> Replies { get; set; } = new List<Sententia>();


    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsEdited { get; set; }


    // Soft-delete.
    public bool IsDeleted { get; set; }
    public Guid? DeletedByCivisId { get; set; } // sabremos si lo ha borrado el dueño o Rector/Magistratus.
    public DateTime? DeletedAt { get; set; }

}
