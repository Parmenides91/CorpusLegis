namespace CorpusLegis.Web.Clients.Base;


[Serializable]
public class ApiValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ApiValidationException(IDictionary<string, string[]> errors)
        : base("Se han producido errores de validación en el servidor.")
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    public ApiValidationException(string message, IDictionary<string, string[]> errors)
        : base(message)
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    public ApiValidationException(string message, Exception innerException, IDictionary<string, string[]> errors)
        : base(message, innerException)
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }


}
