namespace TallerMecanicoFinal.Models;

public sealed class PlateHistoryItemViewModel
{
    public Guid Id { get; init; }

    public string Plate { get; init; } = string.Empty;

    public string OwnerName { get; init; } = string.Empty;

    public string Brand { get; init; } = string.Empty;

    public string Model { get; init; } = string.Empty;

    public DateTime ScheduledAt { get; init; }

    public string StatusLabel { get; init; } = string.Empty;

    public string MechanicName { get; init; } = string.Empty;

    public string? Diagnosis { get; init; }

    public string? Solution { get; init; }
}