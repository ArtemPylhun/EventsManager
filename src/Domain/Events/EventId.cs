namespace Domain.Events;

public record EventId(Guid Value)
{
    public static EventId New() => new(Guid.NewGuid());
    public static EventId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}