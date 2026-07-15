using Microsoft.EntityFrameworkCore;
using TallerMecanicoFinal.Application.Contracts.Repositories;
using TallerMecanicoFinal.Domain.Entities;
using TallerMecanicoFinal.Infrastructure.Persistence;

namespace TallerMecanicoFinal.Infrastructure.Repositories;

public sealed class WorkshopRoleRepository : EfRepository<WorkshopRole>, IWorkshopRoleRepository
{
    public WorkshopRoleRepository(WorkshopDbContext context)
        : base(context)
    {
    }

    public Task<WorkshopRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Role name is required.", nameof(name));
        }

        return Context.Roles.FirstOrDefaultAsync(role => role.Name == name.Trim(), cancellationToken);
    }
}