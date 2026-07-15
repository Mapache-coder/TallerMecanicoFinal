using Microsoft.EntityFrameworkCore;
using TallerMecanicoFinal.Application.Contracts.Repositories;
using TallerMecanicoFinal.Domain.Entities;
using TallerMecanicoFinal.Infrastructure.Persistence;

namespace TallerMecanicoFinal.Infrastructure.Repositories;

public sealed class ServiceAppointmentRepository : EfRepository<ServiceAppointment>, IServiceAppointmentRepository
{
    public ServiceAppointmentRepository(WorkshopDbContext context)
        : base(context)
    {
    }

    public async Task<bool> ExistsAtAsync(DateTime scheduledAt, Guid? excludedAppointmentId = null, CancellationToken cancellationToken = default)
    {
        var normalizedScheduledAt = ServiceAppointment.NormalizeScheduledAt(scheduledAt);

        return await Context.Appointments.AnyAsync(
            appointment => appointment.ScheduledAt == normalizedScheduledAt
                && (!excludedAppointmentId.HasValue || appointment.Id != excludedAppointmentId.Value),
            cancellationToken);
    }
}