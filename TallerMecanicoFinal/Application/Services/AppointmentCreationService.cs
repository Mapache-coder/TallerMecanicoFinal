using TallerMecanicoFinal.Application.Contracts;
using TallerMecanicoFinal.Domain.Entities;

namespace TallerMecanicoFinal.Application.Services;

public sealed class AppointmentCreationService
{
    private readonly IUnitOfWork _unitOfWork;

    public AppointmentCreationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
}