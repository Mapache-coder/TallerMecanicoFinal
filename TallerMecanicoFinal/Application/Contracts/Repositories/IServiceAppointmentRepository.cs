using TallerMecanicoFinal.Domain.Entities;

namespace TallerMecanicoFinal.Application.Contracts.Repositories;

public interface IServiceAppointmentRepository : IRepository<ServiceAppointment>
{
    Task<bool> ExistsAtAsync(DateTime scheduledAt, Guid? excludedAppointmentId = null, CancellationToken cancellationToken = default);
}