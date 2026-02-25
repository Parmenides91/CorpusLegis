using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CorpusLegis.Shared.Validators;

public class ApiValidationException : Exception
{
    //public Dictionary<string, string[]> Errors { get; }

    //public ApiValidationException(Dictionary<string, string[]> errors)
    //    : base("Se han producido errores de validación en la API.")
    //{
    //    Errors = errors;
    //}

    public IReadOnlyDictionary<string, string[]> Errors { get; }
    public ApiValidationException(IDictionary<string, string[]> errors)
        : base("Se han producido errores de validación en la API.")
    {
        Errors = new ReadOnlyDictionary<string, string[]>(new Dictionary<string, string[]>(errors ?? new Dictionary<string, string[]>()));
    }

}
