using Domain.Attendencies;
using Domain.Events;
using Domain.Users;

namespace Domain.Attendances;

public class Attendance
{
    public AttendanceId Id { get; }
    public UserId UserId { get;  }
    public User? User { get;  }
    public EventId EventId { get;  }
    public Event? Event { get;  }
    public DateTime RegistrationDate { get; private set; }

    private Attendance(AttendanceId id, UserId userId, EventId eventId, DateTime registrationDate)
    {
        Id = id;
        UserId = userId;
        EventId = eventId;
        RegistrationDate = registrationDate;
    }

    public static Attendance New(AttendanceId id, UserId userId, EventId eventId, DateTime registrationDate) =>
        new(id, userId, eventId, registrationDate);
}