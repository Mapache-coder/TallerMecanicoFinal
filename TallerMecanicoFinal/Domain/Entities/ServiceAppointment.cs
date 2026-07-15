using TallerMecanicoFinal.Domain.Enums;

namespace TallerMecanicoFinal.Domain.Entities;

public sealed class ServiceAppointment
{
    private ServiceAppointment()
    {
    }

    private ServiceAppointment(
        Guid id,
        string plate,
        string brand,
        string model,
        string ownerDocument,
        string ownerName,
        decimal mileage,
        string ownerPhone,
        DateTime scheduledAt)
    {
        Id = id;
        Plate = plate;
        Brand = brand;
        Model = model;
        OwnerDocument = ownerDocument;
        OwnerName = ownerName;
        Mileage = mileage;
        OwnerPhone = ownerPhone;
        ScheduledAt = scheduledAt;
        Status = AppointmentStatus.Registrado;
    }

    public Guid Id { get; private set; }

    public string Plate { get; private set; } = string.Empty;

    public string Brand { get; private set; } = string.Empty;

    public string Model { get; private set; } = string.Empty;

    public string OwnerDocument { get; private set; } = string.Empty;

    public string OwnerName { get; private set; } = string.Empty;

    public decimal Mileage { get; private set; }

    public string OwnerPhone { get; private set; } = string.Empty;

    public DateTime ScheduledAt { get; private set; }

    public AppointmentStatus Status { get; private set; }

    public Guid? AssignedMechanicId { get; private set; }

    public Mechanic? AssignedMechanic { get; private set; }

    public DateTime? StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public string? Diagnosis { get; private set; }

    public string? Solution { get; private set; }

    public static ServiceAppointment Schedule(
        string plate,
        string brand,
        string model,
        string ownerDocument,
        string ownerName,
        decimal mileage,
        string ownerPhone,
        DateTime scheduledAt)
    {
        ValidateRequiredText(plate, nameof(plate));
        ValidateRequiredText(brand, nameof(brand));
        ValidateRequiredText(model, nameof(model));
        ValidateRequiredText(ownerDocument, nameof(ownerDocument));
        ValidateRequiredText(ownerName, nameof(ownerName));
        ValidateRequiredText(ownerPhone, nameof(ownerPhone));

        if (mileage < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(mileage), "Mileage must be zero or greater.");
        }

        var normalizedScheduledAt = NormalizeScheduledAt(scheduledAt);
        var now = DateTime.Now;

        if (normalizedScheduledAt < now)
        {
            throw new ArgumentException("Appointments cannot be scheduled in the past.", nameof(scheduledAt));
        }

        return new ServiceAppointment(
            Guid.NewGuid(),
            plate.Trim().ToUpperInvariant(),
            brand.Trim(),
            model.Trim(),
            ownerDocument.Trim(),
            ownerName.Trim(),
            mileage,
            ownerPhone.Trim(),
            normalizedScheduledAt);
    }

    public void AssignMechanic(Mechanic mechanic, DateTime assignedAt)
    {
        ArgumentNullException.ThrowIfNull(mechanic);

        if (Status is not AppointmentStatus.Registrado)
        {
            throw new InvalidOperationException("Only registered appointments can be assigned to a mechanic.");
        }

        AssignedMechanicId = mechanic.Id;
        AssignedMechanic = mechanic;
        StartedAt = NormalizeScheduledAt(assignedAt);
        Status = AppointmentStatus.EnProceso;
    }

    public void Complete(string diagnosis, string solution, DateTime completedAt)
    {
        ValidateRequiredText(diagnosis, nameof(diagnosis));
        ValidateRequiredText(solution, nameof(solution));

        if (Status is not AppointmentStatus.EnProceso)
        {
            throw new InvalidOperationException("Only in-process appointments can be completed.");
        }

        Diagnosis = diagnosis.Trim();
        Solution = solution.Trim();
        CompletedAt = NormalizeScheduledAt(completedAt);
        Status = AppointmentStatus.Completado;
    }

    public void Deliver(DateTime deliveredAt)
    {
        if (Status is not (AppointmentStatus.EnProceso or AppointmentStatus.Completado))
        {
            throw new InvalidOperationException("Only in-process or completed appointments can be delivered.");
        }

        CompletedAt ??= NormalizeScheduledAt(deliveredAt);
        Status = AppointmentStatus.Entregado;
    }

    public bool CanBeEditedByMechanic(DateTime currentDateTime)
    {
        if (Status is not AppointmentStatus.Completado || CompletedAt is null)
        {
            return false;
        }

        return currentDateTime <= CompletedAt.Value.AddMonths(1);
    }

    public void ApplyMechanicCorrections(
        string plate,
        string brand,
        string model,
        string ownerDocument,
        string ownerName,
        decimal mileage,
        string ownerPhone,
        string? diagnosis,
        string? solution,
        DateTime currentDateTime)
    {
        if (!CanBeEditedByMechanic(currentDateTime))
        {
            throw new InvalidOperationException("Mechanic edits are no longer allowed for this appointment.");
        }

        ValidateRequiredText(plate, nameof(plate));
        ValidateRequiredText(brand, nameof(brand));
        ValidateRequiredText(model, nameof(model));
        ValidateRequiredText(ownerDocument, nameof(ownerDocument));
        ValidateRequiredText(ownerName, nameof(ownerName));
        ValidateRequiredText(ownerPhone, nameof(ownerPhone));

        Plate = plate.Trim().ToUpperInvariant();
        Brand = brand.Trim();
        Model = model.Trim();
        OwnerDocument = ownerDocument.Trim();
        OwnerName = ownerName.Trim();
        Mileage = mileage;
        OwnerPhone = ownerPhone.Trim();

        if (diagnosis is not null)
        {
            Diagnosis = diagnosis.Trim();
        }

        if (solution is not null)
        {
            Solution = solution.Trim();
        }
    }

    public void UpdateDetails(
        string plate,
        string brand,
        string model,
        string ownerDocument,
        string ownerName,
        decimal mileage,
        string ownerPhone,
        DateTime scheduledAt)
    {
        ValidateRequiredText(plate, nameof(plate));
        ValidateRequiredText(brand, nameof(brand));
        ValidateRequiredText(model, nameof(model));
        ValidateRequiredText(ownerDocument, nameof(ownerDocument));
        ValidateRequiredText(ownerName, nameof(ownerName));
        ValidateRequiredText(ownerPhone, nameof(ownerPhone));

        if (mileage < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(mileage), "Mileage must be zero or greater.");
        }

        Plate = plate.Trim().ToUpperInvariant();
        Brand = brand.Trim();
        Model = model.Trim();
        OwnerDocument = ownerDocument.Trim();
        OwnerName = ownerName.Trim();
        Mileage = mileage;
        OwnerPhone = ownerPhone.Trim();
        ScheduledAt = NormalizeScheduledAt(scheduledAt);
    }

    public static DateTime NormalizeScheduledAt(DateTime scheduledAt)
    {
        return new DateTime(
            scheduledAt.Year,
            scheduledAt.Month,
            scheduledAt.Day,
            scheduledAt.Hour,
            0,
            0,
            scheduledAt.Kind);
    }

    private static void ValidateRequiredText(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{parameterName} is required.", parameterName);
        }
    }
}