using Domain.Attendances;
using Domain.Attendencies;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IAttendanceRepository
{
    Task<Attendance> Add(Attendance attendance, CancellationToken cancellationToken);
    Task<Attendance> Update(Attendance attendance, CancellationToken cancellationToken);
    Task<Attendance> Delete(Attendance attendance, CancellationToken cancellationToken);
}