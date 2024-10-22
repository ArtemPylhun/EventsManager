namespace Domain.Locations;

public record LocationId(Guid Value)
{
    public static LocationId New() => new(Guid.NewGuid());
    public static LocationId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}