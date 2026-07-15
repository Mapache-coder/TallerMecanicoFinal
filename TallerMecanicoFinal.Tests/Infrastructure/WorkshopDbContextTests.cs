using Microsoft.EntityFrameworkCore;
using TallerMecanicoFinal.Domain.Entities;
using TallerMecanicoFinal.Infrastructure.Persistence;

namespace TallerMecanicoFinal.Tests.Infrastructure;

public class WorkshopDbContextTests
{
    [Fact]
    public void Model_DefinesUniqueIndex_OnScheduledAt()
    {
        using var context = CreateContext();

        var entityType = context.Model.FindEntityType(typeof(ServiceAppointment));
        Assert.NotNull(entityType);

        var index = entityType!.GetIndexes().Single(x => x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(ServiceAppointment.ScheduledAt) }));

        Assert.True(index.IsUnique);
    }

    [Fact]
    public void Model_UsesWorkshopSchema()
    {
        using var context = CreateContext();

        Assert.Equal("workshop", context.Model.GetDefaultSchema());
    }

    private static WorkshopDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<WorkshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new WorkshopDbContext(options);
    }
}