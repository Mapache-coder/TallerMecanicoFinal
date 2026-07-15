namespace TallerMecanicoFinal.Application.Services;

public sealed record WorkshopTakeRequest(Guid AppointmentId, Guid MechanicId);