namespace TallerMecanicoFinal.Models;

public sealed class PlateHistoryPageViewModel
{
    public string SearchPlate { get; init; } = string.Empty;

    public bool HasSearch { get; init; }

    public IReadOnlyList<PlateHistoryItemViewModel> Results { get; init; } = Array.Empty<PlateHistoryItemViewModel>();
}