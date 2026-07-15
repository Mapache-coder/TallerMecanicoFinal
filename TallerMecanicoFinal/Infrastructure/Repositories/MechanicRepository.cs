using TallerMecanicoFinal.Application.Contracts.Repositories;
using TallerMecanicoFinal.Domain.Entities;
using TallerMecanicoFinal.Infrastructure.Persistence;

namespace TallerMecanicoFinal.Infrastructure.Repositories;

public sealed class MechanicRepository : EfRepository<Mechanic>, IMechanicRepository
{
    public MechanicRepository(WorkshopDbContext context)
        : base(context)
    {
    }
}