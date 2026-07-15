namespace TallerMecanicoFinal.Models;

public sealed class MonthlyPerformanceRowViewModel
{
    public Guid MechanicId { get; init; }

    public string MechanicName { get; init; } = string.Empty;

    public int CompletedCount { get; init; }
}