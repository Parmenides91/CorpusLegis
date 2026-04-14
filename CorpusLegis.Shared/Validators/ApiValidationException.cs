using System.Collections.ObjectModel;

namespace CorpusLegis.Shared.Validators;

public class ApiValidationException : Exception // TODO: esta clase no debe estar en Shared, sino en el Cliente que la vaya a usar (ya está, ahora hay que revisar dónde se usa ésta y sustituirla).
{

    public IReadOnlyDictionary<string, string[]> Errors { get; }
    public ApiValidationException(IDictionary<string, string[]> errors)
        : base("Se han producido errores de validación en la API.")
    {
        Errors = new ReadOnlyDictionary<string, string[]>(new Dictionary<string, string[]>(errors ?? new Dictionary<string, string[]>()));
    }

}
