using TallerMecanicoFinal.Domain.Entities;

namespace TallerMecanicoFinal.Application.Contracts.Repositories;

public interface IWorkshopRoleRepository : IRepository<WorkshopRole>
{
    Task<WorkshopRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}