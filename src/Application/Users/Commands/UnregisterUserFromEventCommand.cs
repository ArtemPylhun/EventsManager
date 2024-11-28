using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Attendances;
using Domain.Attendencies;
using Domain.Events;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record UnregisterUserFromEventCommand : IRequest<Result<Attendance, UserException>>
{
    public required Guid UserId { get; init; }
    public required Guid EventId { get; init; }
}

public class UnregisterUserFromEventCommandHandler(
    IUserRepository userRepository,
    IUserQueries userQueries,
    IEventQueries eventQueries,
    IAttendanceQueries attendanceQueries,
    IAttendanceRepository attendanceRepository
) : IRequestHandler<UnregisterUserFromEventCommand, Result<Attendance, UserException>>
{
    public async Task<Result<Attendance, UserException>> Handle(UnregisterUserFromEventCommand request,
        CancellationToken cancellationToken)

    {
        var userId = new UserId(request.UserId);
        var eventId = new EventId(request.EventId);
        var existingEvent = await eventQueries.GetById(eventId, cancellationToken);

        return await existingEvent.Match(
            async e =>
            {
                var existingUser = await userQueries.GetById(userId, cancellationToken);
                return await existingUser.Match(
                    async u =>
                    {
                        var attendance = await attendanceQueries.SearchAttendance(eventId, userId, cancellationToken);
                        return await attendance.Match(
                            async a => await UnRegisterUserFromEvent(u, a, cancellationToken),
                            async () => await Task.FromResult<Result<Attendance, UserException>>(
                                new UserAttendanceNotFound(userId, eventId))
                        );
                    },
                    () => Task.FromResult<Result<Attendance, UserException>>(new UserNotFoundException(userId)));
            },
            () => Task.FromResult<Result<Attendance, UserException>>(
                new UserEventNotFoundException(UserId.Empty(), eventId)));
    }

    private async Task<Result<Attendance, UserException>> UnRegisterUserFromEvent(
        User user,
        Attendance attendance,
        CancellationToken cancellationToken)
    {
        try
        {
            return await attendanceRepository.Delete(attendance, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(user.Id, exception);
        }
    }
}