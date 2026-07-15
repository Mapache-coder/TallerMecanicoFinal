using Microsoft.EntityFrameworkCore;
using TallerMecanicoFinal.Application.Contracts.Repositories;
using TallerMecanicoFinal.Application.Services;
using TallerMecanicoFinal.Domain.Entities;
using TallerMecanicoFinal.Infrastructure.Persistence;
using TallerMecanicoFinal.Domain.Enums;

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

    public async Task<IReadOnlyList<ServiceAppointment>> GetHistoryByPlateAsync(string plate, DateTime currentDateTime, CancellationToken cancellationToken = default)
    {
        var normalizedPlate = plate.Trim().ToUpperInvariant();
        var currentDate = currentDateTime.Date;

        return await Context.Appointments
            .AsNoTracking()
            .Include(appointment => appointment.AssignedMechanic)
            .Where(appointment =>
                appointment.Plate == normalizedPlate &&
                (appointment.Status == AppointmentStatus.Completado ||
                 (appointment.Status == AppointmentStatus.Registrado && appointment.ScheduledAt >= currentDate)))
            .OrderByDescending(appointment => appointment.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCompletedCountForMechanicInRangeAsync(Guid mechanicId, DateTime fromInclusive, DateTime toExclusive, CancellationToken cancellationToken = default)
    {
        return await Context.Appointments
            .AsNoTracking()
            .Where(appointment =>
                appointment.AssignedMechanicId == mechanicId &&
                appointment.Status == AppointmentStatus.Completado &&
                appointment.CompletedAt.HasValue &&
                appointment.CompletedAt.Value >= fromInclusive &&
                appointment.CompletedAt.Value < toExclusive)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MechanicMonthlyPerformanceSummary>> GetMonthlyCompletedCountsAsync(DateTime fromInclusive, DateTime toExclusive, CancellationToken cancellationToken = default)
    {
        return await Context.Appointments
            .AsNoTracking()
            .Where(appointment =>
                appointment.Status == AppointmentStatus.Completado &&
                appointment.AssignedMechanicId.HasValue &&
                appointment.CompletedAt.HasValue &&
                appointment.CompletedAt.Value >= fromInclusive &&
                appointment.CompletedAt.Value < toExclusive)
            .GroupBy(appointment => appointment.AssignedMechanicId!.Value)
            .Select(group => new MechanicMonthlyPerformanceSummary(group.Key, group.Count()))
            .ToListAsync(cancellationToken);
    }
}