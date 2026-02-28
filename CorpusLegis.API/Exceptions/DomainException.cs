namespace CorpusLegis.API.Exceptions;

public class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string name, object key)
        : base($"La entidad '{name}' con clave ({key} no ha sido encontrada.") { }
}

public class  UnauthorizedDomainException : DomainException
{
    public UnauthorizedDomainException(string message) : base(message) { }
}

public class  BusinessRuleValidationException : DomainException
{
    public BusinessRuleValidationException(string message) : base(message) { }
}