using TallerMecanicoFinal.Domain.Entities;

namespace TallerMecanicoFinal.Models;

public sealed class WorkshopDashboardViewModel
{
    public Guid? MechanicId { get; init; }

    public bool IsAdmin { get; init; }

    public IReadOnlyList<ServiceAppointment> TodayAppointments { get; init; } = Array.Empty<ServiceAppointment>();

    public ServiceAppointment? FocusAppointment { get; init; }

    public bool CanTakeAnother { get; init; }
}