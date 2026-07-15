using Microsoft.EntityFrameworkCore;
using TallerMecanicoFinal.Application.Contracts.Repositories;
using TallerMecanicoFinal.Infrastructure.Persistence;

namespace TallerMecanicoFinal.Infrastructure.Repositories;

public class EfRepository<T> : IRepository<T> where T : class
{
    protected readonly WorkshopDbContext Context;
    protected readonly DbSet<T> DbSet;

    public EfRepository(WorkshopDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public IQueryable<T> Query()
    {
        return DbSet.AsQueryable();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Remove(T entity)
    {
        DbSet.Remove(entity);
    }
}