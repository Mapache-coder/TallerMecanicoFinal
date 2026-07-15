using Microsoft.EntityFrameworkCore;
using TallerMecanicoFinal.Application.Contracts;
using TallerMecanicoFinal.Application.Services;
using TallerMecanicoFinal.Domain.Entities;
using TallerMecanicoFinal.Infrastructure.Persistence;
using TallerMecanicoFinal.Infrastructure.Repositories;

namespace TallerMecanicoFinal.Tests.Workshop;

public class WorkshopWorkflowServiceTests
{
    [Fact]
    public async Task TakeAsync_Throws_WhenMechanicAlreadyHasActiveAppointment()
    {
        using var context = CreateContext();
        var mechanic = new Mechanic(Guid.NewGuid(), "Carlos Ruiz", "9001");
        var first = ServiceAppointment.Schedule("AAA111", "Toyota", "Corolla", "123", "Ana", 10000, "555", DateTime.Now.AddDays(1));
        var second = ServiceAppointment.Schedule("BBB222", "Nissan", "Versa", "123", "Ana", 12000, "555", DateTime.Now.AddDays(1).AddHours(1));

        context.Mechanics.Add(mechanic);
        context.Appointments.AddRange(first, second);
        await context.SaveChangesAsync();

        first.AssignMechanic(mechanic, DateTime.Now);
        context.Appointments.Update(first);
        await context.SaveChangesAsync();

        var service = new WorkshopWorkflowService(new UnitOfWork(context));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TakeAsync(new WorkshopTakeRequest(second.Id, mechanic.Id), DateTime.Now));

        Assert.Contains("active vehicle", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdateByMechanicAsync_Throws_WhenCompletionWindowExpired()
    {
        using var context = CreateContext();
        var mechanic = new Mechanic(Guid.NewGuid(), "Carlos Ruiz", "9001");
        var appointment = ServiceAppointment.Schedule("CCC333", "Toyota", "Hilux", "123", "Ana", 15000, "555", DateTime.Now.AddDays(1));

        context.Mechanics.Add(mechanic);
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        appointment.AssignMechanic(mechanic, DateTime.Now.AddDays(-2));
        appointment.Complete("Diagnóstico", "Solución", DateTime.Now.AddMonths(-2));
        context.Appointments.Update(appointment);
        await context.SaveChangesAsync();

        var service = new WorkshopWorkflowService(new UnitOfWork(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateByMechanicAsync(
            new WorkshopMechanicEditRequest(
                appointment.Id,
                mechanic.Id,
                "CCC333",
                "Toyota",
                "Hilux",
                "123",
                "Ana",
                15000,
                "555",
                "Ajuste",
                "Cambio"),
            DateTime.Now));
    }

    [Fact]
    public void CanBeEditedByMechanic_ReturnsFalse_WhenMoreThanOneMonthPassed()
    {
        var appointment = ServiceAppointment.Schedule("DDD444", "Toyota", "Yaris", "123", "Ana", 15000, "555", DateTime.Now.AddDays(1));
        appointment.AssignMechanic(new Mechanic(Guid.NewGuid(), "Carlos Ruiz", "9001"), DateTime.Now);
        appointment.Complete("Diagnóstico", "Solución", DateTime.Now.AddMonths(-2));

        Assert.False(appointment.CanBeEditedByMechanic(DateTime.Now));
    }

    private static WorkshopDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<WorkshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new WorkshopDbContext(options);
    }
}