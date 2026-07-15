using Microsoft.EntityFrameworkCore;
using TallerMecanicoFinal.Domain.Entities;

namespace TallerMecanicoFinal.Infrastructure.Persistence;

public sealed class WorkshopDbContext : DbContext
{
    public WorkshopDbContext(DbContextOptions<WorkshopDbContext> options)
        : base(options)
    {
    }

    public DbSet<ServiceAppointment> Appointments => Set<ServiceAppointment>();

    public DbSet<Mechanic> Mechanics => Set<Mechanic>();

    public DbSet<WorkshopRole> Roles => Set<WorkshopRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("workshop");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkshopDbContext).Assembly);
    }
}