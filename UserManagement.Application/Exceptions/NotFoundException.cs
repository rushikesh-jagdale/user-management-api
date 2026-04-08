namespace UserManagement.Application.Exceptions;

public sealed class NotFoundException : ApplicationException
{
    public NotFoundException(string entity, object key)
        : base($"{entity} with key '{key}' was not found.")
    {
    }
}
