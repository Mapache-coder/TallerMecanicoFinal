using TallerMecanicoFinal.Application.Contracts.Repositories;

namespace TallerMecanicoFinal.Application.Contracts;

public interface IUnitOfWork
{
    IServiceAppointmentRepository Appointments { get; }

    IMechanicRepository Mechanics { get; }

    IWorkshopRoleRepository Roles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}