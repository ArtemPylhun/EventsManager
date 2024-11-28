using Domain.Attendances;

namespace Api.Dtos;

public record AttendanceRegisterDto(
    Guid? Id,
    Guid UserId,
    Guid EventId
)
{
    public static AttendanceRegisterDto FromDomainModel(Attendance a) =>
        new(
            Id: a.Id.Value,
            UserId: a.UserId.Value,
            EventId: a.EventId.Value);
}

public record AttendanceUnregisterDto(
    Guid? Id,
    Guid UserId,
    Guid EventId
)
{
    public static AttendanceUnregisterDto FromDomainModel(Attendance a) =>
        new(
            Id: a.Id.Value,
            UserId: a.UserId.Value,
            EventId: a.EventId.Value);
}