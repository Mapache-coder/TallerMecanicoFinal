namespace TallerMecanicoFinal.Models;

public sealed class MonthlyPerformanceViewModel
{
    public DateTime MonthStart { get; init; }

    public DateTime MonthEnd { get; init; }

    public IReadOnlyList<MonthlyPerformanceRowViewModel> Rows { get; init; } = Array.Empty<MonthlyPerformanceRowViewModel>();
}