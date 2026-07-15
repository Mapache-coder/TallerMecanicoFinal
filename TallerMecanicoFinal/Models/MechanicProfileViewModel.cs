namespace TallerMecanicoFinal.Models;

public sealed class MechanicProfileViewModel
{
    public Guid Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string DocumentNumber { get; init; } = string.Empty;

    public int WeeklyCompletedCount { get; init; }

    public DateTime WeekStart { get; init; }

    public DateTime WeekEnd { get; init; }
}