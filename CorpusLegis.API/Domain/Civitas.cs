using CorpusLegis.Shared.Enums;

namespace CorpusLegis.API.Domain;

public class Civitas
{

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Visibilitas Visibility { get; set; } = Visibilitas.Publica;

    public DateTime FoundedAt { get; set; }

    
    public ICollection<Civis> Cives { get; set; } = new List<Civis>(); // skip navigation (sin acceso a los datos de la tabla intermedia).
    public ICollection<CivitasSodalis> Sodales { get; set; } = new List<CivitasSodalis>(); // navegación explícita a la tabla intermedia de los Cives pertenecientes a la Civitas.
    public ICollection<Rogatio> Rogationes { get; set; } = new List<Rogatio>();
    public ICollection<Lex> Leges { get; set; } = new List<Lex>();
    public ICollection<Invitatio> Invitationes { get; set; } = new List<Invitatio>();
}
