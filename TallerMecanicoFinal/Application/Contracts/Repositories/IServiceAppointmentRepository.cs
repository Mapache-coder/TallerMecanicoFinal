using TallerMecanicoFinal.Domain.Entities;
using TallerMecanicoFinal.Application.Services;

namespace TallerMecanicoFinal.Application.Contracts.Repositories;

public interface IServiceAppointmentRepository : IRepository<ServiceAppointment>
{
    Task<bool> ExistsAtAsync(DateTime scheduledAt, Guid? excludedAppointmentId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ServiceAppointment>> GetHistoryByPlateAsync(string plate, DateTime currentDateTime, CancellationToken cancellationToken = default);

    Task<int> GetCompletedCountForMechanicInRangeAsync(Guid mechanicId, DateTime fromInclusive, DateTime toExclusive, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MechanicMonthlyPerformanceSummary>> GetMonthlyCompletedCountsAsync(DateTime fromInclusive, DateTime toExclusive, CancellationToken cancellationToken = default);
}