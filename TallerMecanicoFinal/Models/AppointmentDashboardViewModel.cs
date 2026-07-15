using TallerMecanicoFinal.Domain.Entities;

namespace TallerMecanicoFinal.Models;

public sealed class AppointmentDashboardViewModel
{
    public required IReadOnlyList<ServiceAppointment> Appointments { get; init; }
}