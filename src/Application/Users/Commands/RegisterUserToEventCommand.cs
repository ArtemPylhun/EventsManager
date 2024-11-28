using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Events.Exceptions;
using Application.Users.Exceptions;
using Domain.Attendances;
using Domain.Attendencies;
using Domain.Events;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record RegisterUserToEventCommand : IRequest<Result<Attendance, UserException>>
{
    public required Guid UserId { get; init; }
    public required Guid EventId { get; init; }
}

public class RegisterUserToEventCommandHandler(
    IUserRepository userRepository,
    IUserQueries userQueries,
    IEventQueries eventQueries,
    IAttendanceQueries attendanceQueries,
    IAttendanceRepository attendanceRepository
) : IRequestHandler<RegisterUserToEventCommand, Result<Attendance, UserException>>
{
    public async Task<Result<Attendance, UserException>> Handle(RegisterUserToEventCommand request,
        CancellationToken cancellationToken)

    {
        var userId = new UserId(request.UserId);
        var eventId = new EventId(request.EventId);
        var existingEvent = await eventQueries.GetById(eventId, cancellationToken);

        return await existingEvent.Match(
            async e =>
            {
                if (e.EndDate < DateTime.UtcNow)
                    return await Task.FromResult<Result<Attendance, UserException>>(
                        new UserEventDateHasPassed(userId, eventId));

                if (e.StartDate < DateTime.UtcNow)
                    return await Task.FromResult<Result<Attendance, UserException>>(
                        new UserEventHasAlreadyStarted(userId, eventId));

                var existingUser = await userQueries.GetById(userId, cancellationToken);
                return await existingUser.Match(
                    async u =>
                    {
                        var attendance = await attendanceQueries.SearchAttendance(eventId, userId, cancellationToken);
                        return await attendance.Match(
                            async a => await Task.FromResult<Result<Attendance, UserException>>(
                                new UserEventAlreadyRegistered(userId, eventId)),
                            async () => await RegisterUserToEvent(u, e, cancellationToken)
                        );
                    },
                    () => Task.FromResult<Result<Attendance, UserException>>(new UserNotFoundException(userId)));
            },
            () => Task.FromResult<Result<Attendance, UserException>>(
                new UserEventNotFoundException(UserId.Empty(), eventId)));
    }

    private async Task<Result<Attendance, UserException>> RegisterUserToEvent(
        User user,
        Event @event,
        CancellationToken cancellationToken)
    {
        try
        {
            var attendance = Attendance.New(AttendanceId.New(), user.Id, @event.Id, DateTime.UtcNow);
            return await attendanceRepository.Add(attendance, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(user.Id, exception);
        }
    }
}