namespace TallerMecanicoFinal.Application.Services;

public sealed record AppointmentCreationRequest(
    string Plate,
    string Brand,
    string Model,
    string OwnerDocument,
    string OwnerName,
    decimal Mileage,
    string OwnerPhone,
    DateTime ScheduledAt);