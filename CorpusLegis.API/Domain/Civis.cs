namespace CorpusLegis.API.Domain;

public class Civis
{

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;


    public ICollection<Civitas> Civitates { get; set; } = new List<Civitas>(); // skip navigation (sin acceso a los datos de la tabla intermedia).
    public ICollection<CivitasSodalis> CivitasSodales { get; set; } = new List<CivitasSodalis>(); // navegación explícita a la tabla intermedia de las Civitates a las que pertenece el Civis.

    public ICollection<Invitatio> InvitationesEmissae { get; set; } = new List<Invitatio>(); // invitaciones emitidas por el Civis.
    public ICollection<Invitatio> InvitationesAcceptae { get; set; } = new List<Invitatio>(); // invitaciones aceptadas por el Civis.
}
