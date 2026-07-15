namespace TallerMecanicoFinal.Models;

public sealed class MechanicSummaryCardViewModel
{
    public Guid Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string DocumentNumber { get; init; } = string.Empty;
}