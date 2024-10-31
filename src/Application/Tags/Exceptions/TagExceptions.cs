using Domain.Tags;

namespace Application.Tags.Exceptions;

public class TagException(TagId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public TagId TagId { get; } = id;
}

public class TagNotFoundException(TagId id) : TagException(id, $"Tag under id: {id} not found!");

public class TagAlreadyExistsException(TagId id) : TagException(id, $"Tag already exists: {id}!");

public class TagUnknownException(TagId id, Exception innerException)
    : TagException(id, $"Unknown exception for the Tag under id: {id}!", innerException);