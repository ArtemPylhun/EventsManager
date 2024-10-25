using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Attendances;
using Domain.Attendencies;
using Domain.Events;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class AttendanceRepository(ApplicationDbContext context) : IAttendanceRepository, IAttendanceQueries
{
    public async Task<IReadOnlyList<Attendance>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Attendances
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Option<Attendance>> GetById(AttendanceId id, CancellationToken cancellationToken)
    {
        var entity = await context.Attendances
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return entity == null ? Option.None<Attendance>() : Option.Some(entity);
    }

    public async Task<Option<Attendance>> SearchByUserId(UserId userId, CancellationToken cancellationToken)
    {
        var entity = await context.Attendances
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId , cancellationToken);
        
        return entity == null ? Option.None<Attendance>() : Option.Some(entity);    }

    public async Task<Option<Attendance>> SearchByEventId(EventId eventId, CancellationToken cancellationToken)
    {
        var entity = await context.Attendances
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EventId == eventId, cancellationToken);
        
        return entity == null ? Option.None<Attendance>() : Option.Some(entity);    }

    public async Task<Option<Attendance>> SearchAttendance(EventId eventId, UserId userId , CancellationToken cancellationToken)
    {
        var entity = await context.Attendances
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EventId ==  eventId && x.UserId == userId , cancellationToken);
        
        return entity == null ? Option.None<Attendance>() : Option.Some(entity);
    }

    public async Task<Attendance> Add(Attendance attendance, CancellationToken cancellationToken)
    {
        await context.Attendances.AddAsync(attendance, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return attendance;
    }

    public async Task<Attendance> Update(Attendance attendance, CancellationToken cancellationToken)
    {
        context.Attendances.Update(attendance);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return attendance;
    }

    public async Task<Attendance> Delete(Attendance attendance, CancellationToken cancellationToken)
    {
        context.Attendances.Remove(attendance);
        
        await context.SaveChangesAsync(cancellationToken);

        return attendance;
    }
}