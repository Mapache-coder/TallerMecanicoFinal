namespace TallerMecanicoFinal.Domain.Entities;

public sealed class WorkshopRole
{
    private WorkshopRole()
    {
    }

    public WorkshopRole(Guid id, string name)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Role identifier is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Role name is required.", nameof(name));
        }

        Id = id;
        Name = name.Trim();
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;
}