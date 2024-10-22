namespace Domain.Attendencies;

public record AttendanceId(Guid Value)
{
    public static AttendanceId New() => new(Guid.NewGuid());
    public static AttendanceId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}