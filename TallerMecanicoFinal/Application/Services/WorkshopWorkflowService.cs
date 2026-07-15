using Microsoft.EntityFrameworkCore;
using TallerMecanicoFinal.Application.Contracts;
using TallerMecanicoFinal.Domain.Entities;
using TallerMecanicoFinal.Domain.Enums;
using TallerMecanicoFinal.Models;

namespace TallerMecanicoFinal.Application.Services;

public sealed class WorkshopWorkflowService
{
    private readonly IUnitOfWork _unitOfWork;

    public WorkshopWorkflowService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<WorkshopDashboardViewModel> GetDashboardAsync(Guid? mechanicId, bool isAdmin, DateTime currentDateTime, CancellationToken cancellationToken = default)
    {
        var today = currentDateTime.Date;

        var todaysAppointments = await _unitOfWork.Appointments
            .Query()
            .Include(appointment => appointment.AssignedMechanic)
            .Where(appointment => appointment.ScheduledAt.Date == today && appointment.Status == AppointmentStatus.Registrado)
            .OrderBy(appointment => appointment.ScheduledAt)
            .ToListAsync(cancellationToken);

        ServiceAppointment? focusAppointment = null;
        bool canTakeAnother = true;

        if (mechanicId.HasValue)
        {
            var mechanicAppointments = _unitOfWork.Appointments
                .Query()
                .Include(appointment => appointment.AssignedMechanic)
                .Where(appointment => appointment.AssignedMechanicId == mechanicId.Value)
                .OrderByDescending(appointment => appointment.ScheduledAt);

            focusAppointment = await mechanicAppointments.FirstOrDefaultAsync(cancellationToken)
                ?? await mechanicAppointments.FirstOrDefaultAsync(appointment => appointment.Status == AppointmentStatus.Completado, cancellationToken);

            canTakeAnother = !await _unitOfWork.Appointments.Query().AnyAsync(
                appointment => appointment.AssignedMechanicId == mechanicId.Value && appointment.Status == AppointmentStatus.EnProceso,
                cancellationToken);
        }

        return new WorkshopDashboardViewModel
        {
            MechanicId = mechanicId,
            IsAdmin = isAdmin,
            TodayAppointments = todaysAppointments,
            FocusAppointment = focusAppointment,
            CanTakeAnother = canTakeAnother
        };
    }

    public async Task TakeAsync(WorkshopTakeRequest request, DateTime currentDateTime, CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken)
            ?? throw new InvalidOperationException("Appointment not found.");

        if (appointment.Status != AppointmentStatus.Registrado)
        {
            throw new InvalidOperationException("Only registered appointments can be assigned.");
        }

        var mechanic = await _unitOfWork.Mechanics.GetByIdAsync(request.MechanicId, cancellationToken)
            ?? throw new InvalidOperationException("Mechanic not found.");

        var mechanicAlreadyActive = await _unitOfWork.Appointments.Query().AnyAsync(
            item => item.AssignedMechanicId == request.MechanicId && item.Status == AppointmentStatus.EnProceso,
            cancellationToken);

        if (mechanicAlreadyActive)
        {
            throw new InvalidOperationException("The mechanic already has one active vehicle in progress.");
        }

        appointment.AssignMechanic(mechanic, currentDateTime);
        _unitOfWork.Appointments.Update(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(WorkshopStatusRequest request, DateTime currentDateTime, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken)
            ?? throw new InvalidOperationException("Appointment not found.");

        if (!isAdmin)
        {
            if (appointment.AssignedMechanicId != request.MechanicId)
            {
                throw new InvalidOperationException("The mechanic can only change the status of the assigned order.");
            }

            if (appointment.Status == AppointmentStatus.Completado && !appointment.CanBeEditedByMechanic(currentDateTime))
            {
                throw new InvalidOperationException("Mechanic changes are no longer allowed after the one-month window.");
            }
        }

        switch (request.Status)
        {
            case AppointmentStatus.Completado:
                appointment.Complete(request.Diagnosis ?? string.Empty, request.Solution ?? string.Empty, currentDateTime);
                break;
            case AppointmentStatus.Entregado:
                appointment.Deliver(currentDateTime);
                break;
            default:
                throw new InvalidOperationException("Only completed or delivered statuses can be applied from this flow.");
        }

        _unitOfWork.Appointments.Update(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateByMechanicAsync(WorkshopMechanicEditRequest request, DateTime currentDateTime, CancellationToken cancellationToken = default)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken)
            ?? throw new InvalidOperationException("Appointment not found.");

        if (appointment.AssignedMechanicId != request.MechanicId)
        {
            throw new InvalidOperationException("The mechanic can only edit the assigned order.");
        }

        if (!appointment.CanBeEditedByMechanic(currentDateTime))
        {
            throw new InvalidOperationException("Mechanic edits are no longer allowed for this appointment.");
        }

        appointment.ApplyMechanicCorrections(
            request.Plate,
            request.Brand,
            request.Model,
            request.OwnerDocument,
            request.OwnerName,
            request.Mileage,
            request.OwnerPhone,
            request.Diagnosis,
            request.Solution,
            currentDateTime);

        _unitOfWork.Appointments.Update(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}