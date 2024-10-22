namespace Domain.Tags;

public record TagId(Guid Value)
{
    public static TagId New() => new(Guid.NewGuid());
    public static TagId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}