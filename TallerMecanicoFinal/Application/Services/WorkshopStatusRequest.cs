using TallerMecanicoFinal.Domain.Enums;

namespace TallerMecanicoFinal.Application.Services;

public sealed record WorkshopStatusRequest(
    Guid AppointmentId,
    Guid MechanicId,
    AppointmentStatus Status,
    string? Diagnosis,
    string? Solution);