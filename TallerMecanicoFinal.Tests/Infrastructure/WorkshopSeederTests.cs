using Microsoft.EntityFrameworkCore;
using TallerMecanicoFinal.Domain.Entities;
using TallerMecanicoFinal.Infrastructure.Persistence;
using TallerMecanicoFinal.Infrastructure.Seeding;

namespace TallerMecanicoFinal.Tests.Infrastructure;

public class WorkshopSeederTests
{
    [Fact]
    public async Task SeedAsync_InsertsAdministratorAndMechanicRoles()
    {
        using var context = CreateContext();

        await WorkshopSeeder.SeedAsync(context);

        var roles = await context.Roles.OrderBy(x => x.Name).ToListAsync();

        Assert.Collection(roles,
            role => Assert.Equal("Administrador", role.Name),
            role => Assert.Equal("Mecánico", role.Name));
    }

    private static WorkshopDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<WorkshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new WorkshopDbContext(options);
    }
}