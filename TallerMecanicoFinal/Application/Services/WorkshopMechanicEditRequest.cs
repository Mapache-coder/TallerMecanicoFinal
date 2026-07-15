namespace TallerMecanicoFinal.Application.Services;

public sealed record WorkshopMechanicEditRequest(
    Guid AppointmentId,
    Guid MechanicId,
    string Plate,
    string Brand,
    string Model,
    string OwnerDocument,
    string OwnerName,
    decimal Mileage,
    string OwnerPhone,
    string? Diagnosis,
    string? Solution);