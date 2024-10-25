using Domain.Attendances;
using Domain.Attendencies;
using Domain.Events;
using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IAttendanceQueries
{
    Task<Option<Attendance>> GetById(AttendanceId id, CancellationToken cancellationToken);
    Task<Option<Attendance>> SearchByUserId(UserId userId, CancellationToken cancellationToken);
    Task<Option<Attendance>> SearchByEventId(EventId eventId, CancellationToken cancellationToken);
    Task<Option<Attendance>> SearchAttendance(EventId eventId, UserId userId, CancellationToken cancellationToken);
}