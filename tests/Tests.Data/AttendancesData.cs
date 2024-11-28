using Domain.Attendances;
using Domain.Attendencies;
using Domain.Events;
using Domain.Users;

namespace Tests.Data;

public class AttendancesData
{
    public static Attendance MainAttendance(UserId userId, EventId eventId, DateTime registrationDate) =>
        Attendance.New(AttendanceId.New(), userId, eventId, registrationDate);

    public static Attendance PastAttendance(UserId userId, EventId eventId, DateTime registrationDate) =>
        Attendance.New(AttendanceId.New(), userId, eventId, registrationDate);

    public static Attendance FutureAttendance(UserId userId, EventId eventId, DateTime registrationDate) =>
        Attendance.New(AttendanceId.New(), userId, eventId, registrationDate);
}