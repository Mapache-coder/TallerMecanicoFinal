using Microsoft.AspNetCore.Mvc;
using TallerMecanicoFinal.Application.Services;
using TallerMecanicoFinal.Models;

namespace TallerMecanicoFinal.Controllers;

public class ReportsController : Controller
{
    private readonly WorkshopReportingService _reportingService;

    public ReportsController(WorkshopReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> History(string? plate, CancellationToken cancellationToken = default)
    {
        var model = await _reportingService.SearchHistoryAsync(plate, DateTime.Now, cancellationToken);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Mechanics(CancellationToken cancellationToken = default)
    {
        var model = await _reportingService.GetMechanicsDirectoryAsync(cancellationToken);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> MechanicProfile(Guid id, CancellationToken cancellationToken = default)
    {
        var model = await _reportingService.GetMechanicProfileAsync(id, DateTime.Now, cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Performance(CancellationToken cancellationToken = default)
    {
        var model = await _reportingService.GetMonthlyPerformanceAsync(DateTime.Now, cancellationToken);
        return View(model);
    }
}