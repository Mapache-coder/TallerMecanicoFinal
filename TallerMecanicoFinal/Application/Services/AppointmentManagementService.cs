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

        if (await _unitOfWork.Appointments.ExistsAtAsync(appointment.ScheduledAt, null, cancellationToken))
        {
            throw new InvalidOperationException("There is already an appointment scheduled for the selected date and hour.");
        }

        await _unitOfWork.Appointments.AddAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return appointment;
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

        appointment.UpdateDetails(
            request.Plate,
            request.Brand,
            request.Model,
            request.OwnerDocument,
            request.OwnerName,
            request.Mileage,
            request.OwnerPhone,
            request.ScheduledAt);

        _unitOfWork.Appointments.Update(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return appointment;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Appointment not found.");

        _unitOfWork.Appointments.Remove(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

}