using System.ComponentModel.DataAnnotations;
using TallerMecanicoFinal.Application.Services;
using TallerMecanicoFinal.Domain.Entities;

namespace TallerMecanicoFinal.Models;

public sealed class WorkshopTakeFormViewModel
{
    [Required]
    public Guid AppointmentId { get; set; }

    [Required]
    public Guid MechanicId { get; set; }

    public bool CanTakeAnother { get; set; }

    public bool IsAdmin { get; set; }

    public string Plate { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public static WorkshopTakeFormViewModel FromEntity(ServiceAppointment appointment, Guid mechanicId, bool canTakeAnother, bool isAdmin)
    {
        return new WorkshopTakeFormViewModel
        {
            AppointmentId = appointment.Id,
            MechanicId = mechanicId,
            CanTakeAnother = canTakeAnother,
            IsAdmin = isAdmin,
            Plate = appointment.Plate,
            OwnerName = appointment.OwnerName
        };
    }

    public WorkshopTakeRequest ToRequest()
    {
        return new WorkshopTakeRequest(AppointmentId, MechanicId);
    }
}