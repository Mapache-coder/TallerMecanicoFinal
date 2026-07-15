using Microsoft.AspNetCore.Mvc;
using TallerMecanicoFinal.Application.Services;
using TallerMecanicoFinal.Domain.Entities;

namespace TallerMecanicoFinal.Controllers;

public class AppointmentsController : Controller
{
    private readonly AppointmentManagementService _appointmentManagementService;

    public AppointmentsController(AppointmentManagementService appointmentManagementService)
    {
        _appointmentManagementService = appointmentManagementService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var appointments = await _appointmentManagementService.GetAllAsync(cancellationToken);
        return View(appointments);
    }

    public IActionResult Create()
    {
        return PartialView("_AppointmentForm", AppointmentFormViewModel.Empty());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppointmentFormViewModel appointmentModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_AppointmentForm", appointmentModel);
        }

        await _appointmentManagementService.CreateAsync(appointmentModel.ToCreationRequest(), cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentManagementService.GetByIdAsync(id, cancellationToken);
        if (appointment is null)
        {
            return NotFound();
        }

        return PartialView("_AppointmentForm", AppointmentFormViewModel.FromEntity(appointment));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AppointmentFormViewModel appointmentModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_AppointmentForm", appointmentModel);
        }

        await _appointmentManagementService.UpdateAsync(appointmentModel.ToUpdateRequest(), cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _appointmentManagementService.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}