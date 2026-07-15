using Microsoft.AspNetCore.Mvc;
using TallerMecanicoFinal.Application.Services;
using TallerMecanicoFinal.Domain.Enums;
using TallerMecanicoFinal.Models;

namespace TallerMecanicoFinal.Controllers;

public class WorkshopController : Controller
{
    private readonly WorkshopWorkflowService _workflowService;
    private readonly AppointmentManagementService _appointmentManagementService;

    public WorkshopController(WorkshopWorkflowService workflowService, AppointmentManagementService appointmentManagementService)
    {
        _workflowService = workflowService;
        _appointmentManagementService = appointmentManagementService;
    }

    public async Task<IActionResult> Index(Guid? mechanicId, bool isAdmin = false, CancellationToken cancellationToken = default)
    {
        var model = await _workflowService.GetDashboardAsync(mechanicId, isAdmin, DateTime.Now, cancellationToken);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Board(Guid? mechanicId, bool isAdmin = false, CancellationToken cancellationToken = default)
    {
        var model = await _workflowService.GetDashboardAsync(mechanicId, isAdmin, DateTime.Now, cancellationToken);
        return PartialView("_WorkshopBoard", model);
    }

    [HttpGet]
    public async Task<IActionResult> Take(Guid id, Guid mechanicId, bool isAdmin = false, CancellationToken cancellationToken = default)
    {
        var dashboard = await _workflowService.GetDashboardAsync(mechanicId, isAdmin, DateTime.Now, cancellationToken);
        var appointment = dashboard.TodayAppointments.FirstOrDefault(item => item.Id == id);
        if (appointment is null)
        {
            return NotFound();
        }

        return PartialView("_TakeForm", WorkshopTakeFormViewModel.FromEntity(appointment, mechanicId, dashboard.CanTakeAnother, dashboard.IsAdmin));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Take(WorkshopTakeFormViewModel model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_TakeForm", model);
        }

        await _workflowService.TakeAsync(model.ToRequest(), DateTime.Now, cancellationToken);
        return Json(new { refreshUrl = Url.Action(nameof(Board), new { mechanicId = model.MechanicId, isAdmin = model.IsAdmin }) });
    }

    [HttpGet]
    public async Task<IActionResult> ChangeStatus(Guid id, Guid mechanicId, bool isAdmin = false, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentManagementService.GetByIdAsync(id, cancellationToken);
        if (appointment is null)
        {
            return NotFound();
        }

        var canEdit = isAdmin || (appointment.AssignedMechanicId == mechanicId && appointment.CanBeEditedByMechanic(DateTime.Now));
        return PartialView("_StatusForm", WorkshopStatusFormViewModel.FromEntity(appointment, mechanicId, isAdmin, canEdit));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(WorkshopStatusFormViewModel model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_StatusForm", model);
        }

        await _workflowService.ChangeStatusAsync(model.ToRequest(), DateTime.Now, model.IsAdmin, cancellationToken);
        return Json(new { refreshUrl = Url.Action(nameof(Board), new { mechanicId = model.MechanicId, isAdmin = model.IsAdmin }) });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, Guid? mechanicId, bool isAdmin = false, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentManagementService.GetByIdAsync(id, cancellationToken);
        if (appointment is null)
        {
            return NotFound();
        }

        var isMechanicEditable = mechanicId.HasValue
            && appointment.AssignedMechanicId == mechanicId.Value
            && appointment.CanBeEditedByMechanic(DateTime.Now);

        var form = AppointmentFormViewModel.FromEntity(appointment);
        form.IsReadOnly = !isAdmin && !isMechanicEditable;
        return PartialView("_WorkshopEditForm", form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AppointmentFormViewModel appointmentModel, Guid? mechanicId, bool isAdmin = false, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            appointmentModel.IsReadOnly = !isAdmin;
            return PartialView("_WorkshopEditForm", appointmentModel);
        }

        if (isAdmin)
        {
            await _appointmentManagementService.UpdateAsync(appointmentModel.ToUpdateRequest(), cancellationToken);
        }
        else
        {
            if (!mechanicId.HasValue)
            {
                throw new InvalidOperationException("Mechanic identifier is required for workshop edits.");
            }

            await _workflowService.UpdateByMechanicAsync(
                new WorkshopMechanicEditRequest(
                    appointmentModel.Id ?? throw new InvalidOperationException("Appointment identifier is required."),
                    mechanicId.Value,
                    appointmentModel.Plate,
                    appointmentModel.Brand,
                    appointmentModel.Model,
                    appointmentModel.OwnerDocument,
                    appointmentModel.OwnerName,
                    appointmentModel.Mileage,
                    appointmentModel.OwnerPhone,
                    null,
                    null),
                DateTime.Now,
                cancellationToken);
        }

        return Json(new { refreshUrl = Url.Action(nameof(Board), new { mechanicId, isAdmin }) });
    }
}