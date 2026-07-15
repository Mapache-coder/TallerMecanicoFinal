using Microsoft.EntityFrameworkCore;
using TallerMecanicoFinal.Domain.Entities;
using TallerMecanicoFinal.Infrastructure.Persistence;

namespace TallerMecanicoFinal.Infrastructure.Seeding;

public static class WorkshopSeeder
{
    private static readonly IReadOnlyCollection<WorkshopRole> SeedRoles = new[]
    {
        new WorkshopRole(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Administrador"),
        new WorkshopRole(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Mecánico")
    };

    public static async Task SeedAsync(WorkshopDbContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        foreach (var role in SeedRoles)
        {
            var alreadyExists = await context.Roles.AnyAsync(existing => existing.Name == role.Name, cancellationToken);
            if (!alreadyExists)
            {
                await context.Roles.AddAsync(role, cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}