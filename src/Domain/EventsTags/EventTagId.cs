namespace Domain.EventsTags;

public record EventTagId(Guid Value)
{
    public static EventTagId New() => new(Guid.NewGuid());
    public static EventTagId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}