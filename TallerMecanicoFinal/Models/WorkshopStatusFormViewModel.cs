using System.ComponentModel.DataAnnotations;
using TallerMecanicoFinal.Application.Services;
using TallerMecanicoFinal.Domain.Entities;
using TallerMecanicoFinal.Domain.Enums;

namespace TallerMecanicoFinal.Models;

public sealed class WorkshopStatusFormViewModel
{
    [Required]
    public Guid AppointmentId { get; set; }

    [Required]
    public Guid MechanicId { get; set; }

    public bool IsAdmin { get; set; }

    public bool CanEdit { get; set; }

    [Required]
    public AppointmentStatus Status { get; set; }

    [StringLength(2000)]
    public string? Diagnosis { get; set; }

    [StringLength(2000)]
    public string? Solution { get; set; }

    public string Plate { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public static WorkshopStatusFormViewModel FromEntity(ServiceAppointment appointment, Guid mechanicId, bool isAdmin, bool canEdit)
    {
        return new WorkshopStatusFormViewModel
        {
            AppointmentId = appointment.Id,
            MechanicId = mechanicId,
            IsAdmin = isAdmin,
            CanEdit = canEdit,
            Status = appointment.Status == AppointmentStatus.EnProceso ? AppointmentStatus.Completado : AppointmentStatus.Entregado,
            Diagnosis = appointment.Diagnosis,
            Solution = appointment.Solution,
            Plate = appointment.Plate,
            OwnerName = appointment.OwnerName
        };
    }

    public WorkshopStatusRequest ToRequest()
    {
        return new WorkshopStatusRequest(AppointmentId, MechanicId, Status, Diagnosis, Solution);
    }
}