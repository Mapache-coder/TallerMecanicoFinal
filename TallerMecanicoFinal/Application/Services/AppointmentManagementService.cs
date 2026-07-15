using Microsoft.EntityFrameworkCore;
using TallerMecanicoFinal.Application.Contracts;
using TallerMecanicoFinal.Domain.Entities;

namespace TallerMecanicoFinal.Application.Services;

public sealed class AppointmentManagementService
{
    private readonly IUnitOfWork _unitOfWork;

    public AppointmentManagementService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<List<ServiceAppointment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Appointments
            .Query()
            .Include(appointment => appointment.AssignedMechanic)
            .OrderByDescending(appointment => appointment.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceAppointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Appointments
            .Query()
            .Include(appointment => appointment.AssignedMechanic)
            .FirstOrDefaultAsync(appointment => appointment.Id == id, cancellationToken);
    }

    public async Task<ServiceAppointment> CreateAsync(AppointmentCreationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var appointment = ServiceAppointment.Schedule(
            request.Plate,
            request.Brand,
            request.Model,
            request.OwnerDocument,
            request.OwnerName,
            request.Mileage,
            request.OwnerPhone,
            request.ScheduledAt);

        return await PersistAsync(appointment, cancellationToken);
    }

    public async Task<ServiceAppointment> UpdateAsync(AppointmentUpdateRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Appointment not found.");

        var normalizedScheduledAt = ServiceAppointment.NormalizeScheduledAt(request.ScheduledAt);

        if (await _unitOfWork.Appointments.ExistsAtAsync(normalizedScheduledAt, request.Id, cancellationToken))
        {
            throw new InvalidOperationException("There is already an appointment scheduled for the selected date and hour.");
        }

        appointment.ApplyMechanicCorrections(
            request.Plate,
            request.Brand,
            request.Model,
            request.OwnerDocument,
            request.OwnerName,
            request.Mileage,
            request.OwnerPhone,
            null,
            null,
            DateTime.Now);

        return await PersistAsync(appointment, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Appointment not found.");

        _unitOfWork.Appointments.Remove(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<ServiceAppointment> PersistAsync(ServiceAppointment appointment, CancellationToken cancellationToken)
    {
        var existsAtSameSlot = await _unitOfWork.Appointments.ExistsAtAsync(appointment.ScheduledAt, appointment.Id == Guid.Empty ? null : appointment.Id, cancellationToken);
        if (existsAtSameSlot)
        {
            throw new InvalidOperationException("There is already an appointment scheduled for the selected date and hour.");
        }

        if (appointment.Id == Guid.Empty)
        {
            await _unitOfWork.Appointments.AddAsync(appointment, cancellationToken);
        }
        else
        {
            _unitOfWork.Appointments.Update(appointment);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return appointment;
    }
}