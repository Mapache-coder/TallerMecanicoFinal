using Microsoft.EntityFrameworkCore;
using TallerMecanicoFinal.Application.Contracts;
using TallerMecanicoFinal.Domain.Enums;
using TallerMecanicoFinal.Models;

namespace TallerMecanicoFinal.Application.Services;

public sealed class WorkshopReportingService
{
    private readonly IUnitOfWork _unitOfWork;

    public WorkshopReportingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PlateHistoryPageViewModel> SearchHistoryAsync(string? plate, DateTime currentDateTime, CancellationToken cancellationToken = default)
    {
        var normalizedPlate = NormalizePlate(plate);
        var results = Array.Empty<PlateHistoryItemViewModel>();

        if (!string.IsNullOrWhiteSpace(normalizedPlate))
        {
            var appointments = await _unitOfWork.Appointments.GetHistoryByPlateAsync(normalizedPlate, currentDateTime, cancellationToken);
            results = appointments.Select(appointment => new PlateHistoryItemViewModel
            {
                Id = appointment.Id,
                Plate = appointment.Plate,
                OwnerName = appointment.OwnerName,
                Brand = appointment.Brand,
                Model = appointment.Model,
                ScheduledAt = appointment.ScheduledAt,
                StatusLabel = appointment.Status == AppointmentStatus.Registrado ? "Programada" : appointment.Status.ToString(),
                MechanicName = appointment.AssignedMechanic?.FullName ?? "Sin mecánico",
                Diagnosis = appointment.Diagnosis,
                Solution = appointment.Solution
            }).ToArray();
        }

        return new PlateHistoryPageViewModel
        {
            SearchPlate = plate ?? string.Empty,
            Results = results,
            HasSearch = !string.IsNullOrWhiteSpace(normalizedPlate)
        };
    }

    public async Task<MechanicDirectoryViewModel> GetMechanicsDirectoryAsync(CancellationToken cancellationToken = default)
    {
        var mechanics = await _unitOfWork.Mechanics
            .Query()
            .AsNoTracking()
            .OrderBy(mechanic => mechanic.FullName)
            .Select(mechanic => new MechanicSummaryCardViewModel
            {
                Id = mechanic.Id,
                FullName = mechanic.FullName,
                DocumentNumber = mechanic.DocumentNumber
            })
            .ToArrayAsync(cancellationToken);

        return new MechanicDirectoryViewModel
        {
            Mechanics = mechanics
        };
    }

    public async Task<MechanicProfileViewModel?> GetMechanicProfileAsync(Guid mechanicId, DateTime currentDateTime, CancellationToken cancellationToken = default)
    {
        var mechanic = await _unitOfWork.Mechanics.GetByIdAsync(mechanicId, cancellationToken);
        if (mechanic is null)
        {
            return null;
        }

        var weekStart = GetStartOfWeek(currentDateTime);
        var weekEnd = weekStart.AddDays(7);
        var weeklyCompletedCount = await _unitOfWork.Appointments.GetCompletedCountForMechanicInRangeAsync(mechanicId, weekStart, weekEnd, cancellationToken);

        return new MechanicProfileViewModel
        {
            Id = mechanic.Id,
            FullName = mechanic.FullName,
            DocumentNumber = mechanic.DocumentNumber,
            WeeklyCompletedCount = weeklyCompletedCount,
            WeekStart = weekStart,
            WeekEnd = weekEnd
        };
    }

    public async Task<MonthlyPerformanceViewModel> GetMonthlyPerformanceAsync(DateTime currentDateTime, CancellationToken cancellationToken = default)
    {
        var monthStart = new DateTime(currentDateTime.Year, currentDateTime.Month, 1);
        var nextMonthStart = monthStart.AddMonths(1);

        var mechanics = await _unitOfWork.Mechanics
            .Query()
            .AsNoTracking()
            .OrderBy(mechanic => mechanic.FullName)
            .ToArrayAsync(cancellationToken);

        var counts = await _unitOfWork.Appointments.GetMonthlyCompletedCountsAsync(monthStart, nextMonthStart, cancellationToken);
        var countMap = counts.ToDictionary(item => item.MechanicId, item => item.CompletedCount);

        return new MonthlyPerformanceViewModel
        {
            MonthStart = monthStart,
            MonthEnd = nextMonthStart,
            Rows = mechanics.Select(mechanic => new MonthlyPerformanceRowViewModel
            {
                MechanicId = mechanic.Id,
                MechanicName = mechanic.FullName,
                CompletedCount = countMap.TryGetValue(mechanic.Id, out var count) ? count : 0
            }).ToArray()
        };
    }

    private static string NormalizePlate(string? plate)
    {
        return string.IsNullOrWhiteSpace(plate)
            ? string.Empty
            : plate.Trim().ToUpperInvariant();
    }

    private static DateTime GetStartOfWeek(DateTime currentDateTime)
    {
        var date = currentDateTime.Date;
        var offset = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return date.AddDays(-offset);
    }
}