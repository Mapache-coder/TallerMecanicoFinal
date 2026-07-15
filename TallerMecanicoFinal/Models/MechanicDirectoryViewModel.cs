namespace TallerMecanicoFinal.Models;

public sealed class MechanicDirectoryViewModel
{
    public IReadOnlyList<MechanicSummaryCardViewModel> Mechanics { get; init; } = Array.Empty<MechanicSummaryCardViewModel>();
}