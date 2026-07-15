namespace TallerMecanicoFinal.Domain.Entities;

public sealed class Mechanic
{
    private Mechanic()
    {
    }

    public Mechanic(Guid id, string fullName, string documentNumber)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Mechanic identifier is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Mechanic full name is required.", nameof(fullName));
        }

        if (string.IsNullOrWhiteSpace(documentNumber))
        {
            throw new ArgumentException("Mechanic document number is required.", nameof(documentNumber));
        }

        Id = id;
        FullName = fullName.Trim();
        DocumentNumber = documentNumber.Trim();
    }

    public Guid Id { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public string DocumentNumber { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;

    public ICollection<ServiceAppointment> AssignedAppointments { get; private set; } = new List<ServiceAppointment>();
}