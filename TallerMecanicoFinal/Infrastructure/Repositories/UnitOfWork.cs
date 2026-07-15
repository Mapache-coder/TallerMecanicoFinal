using TallerMecanicoFinal.Application.Contracts;
using TallerMecanicoFinal.Application.Contracts.Repositories;
using TallerMecanicoFinal.Infrastructure.Persistence;

namespace TallerMecanicoFinal.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(WorkshopDbContext context)
    {
        Appointments = new ServiceAppointmentRepository(context);
        Mechanics = new MechanicRepository(context);
        Roles = new WorkshopRoleRepository(context);
        Context = context;
    }

    private WorkshopDbContext Context { get; }

    public IServiceAppointmentRepository Appointments { get; }

    public IMechanicRepository Mechanics { get; }

    public IWorkshopRoleRepository Roles { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Context.SaveChangesAsync(cancellationToken);
    }
}