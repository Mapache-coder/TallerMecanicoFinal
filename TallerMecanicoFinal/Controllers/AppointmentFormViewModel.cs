using System.ComponentModel.DataAnnotations;
using TallerMecanicoFinal.Application.Services;
using TallerMecanicoFinal.Domain.Entities;

namespace TallerMecanicoFinal.Controllers;

public sealed class AppointmentFormViewModel
{
    public Guid? Id { get; set; }

    public bool IsReadOnly { get; set; }

    [Required]
    [StringLength(20)]
    public string Plate { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Model { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string OwnerDocument { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string OwnerName { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Mileage { get; set; }

    [Required]
    [StringLength(30)]
    public string OwnerPhone { get; set; } = string.Empty;

    [Required]
    public DateTime ScheduledAt { get; set; }

    public static AppointmentFormViewModel Empty()
    {
        return new AppointmentFormViewModel
        {
            ScheduledAt = DateTime.Today.AddHours(8)
        };
    }

    public static AppointmentFormViewModel FromEntity(ServiceAppointment appointment)
    {
        return new AppointmentFormViewModel
        {
            Id = appointment.Id,
            Plate = appointment.Plate,
            Brand = appointment.Brand,
            Model = appointment.Model,
            OwnerDocument = appointment.OwnerDocument,
            OwnerName = appointment.OwnerName,
            Mileage = appointment.Mileage,
            OwnerPhone = appointment.OwnerPhone,
            ScheduledAt = appointment.ScheduledAt
        };
    }

    public AppointmentCreationRequest ToCreationRequest()
    {
        return new AppointmentCreationRequest(Plate, Brand, Model, OwnerDocument, OwnerName, Mileage, OwnerPhone, ScheduledAt);
    }

    public AppointmentUpdateRequest ToUpdateRequest()
    {
        if (!Id.HasValue)
        {
            throw new InvalidOperationException("Appointment identifier is required.");
        }

        return new AppointmentUpdateRequest(Id.Value, Plate, Brand, Model, OwnerDocument, OwnerName, Mileage, OwnerPhone, ScheduledAt);
    }
}