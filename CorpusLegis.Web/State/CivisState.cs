namespace CorpusLegis.Web.State;

public class CivisState
{
    // GUID de Sempronio por defecto.
    public Guid CurrentCivisId { get; set; } = Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"); // Sempronio

    public event Action? OnChange;

    public void SetCivis(Guid civisId)
    {
        CurrentCivisId = civisId;
        OnChange?.Invoke();
    }

}
