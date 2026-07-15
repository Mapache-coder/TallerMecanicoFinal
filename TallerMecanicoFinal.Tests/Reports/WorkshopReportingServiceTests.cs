using Microsoft.EntityFrameworkCore;
using TallerMecanicoFinal.Application.Services;
using TallerMecanicoFinal.Domain.Entities;
using TallerMecanicoFinal.Domain.Enums;
using TallerMecanicoFinal.Infrastructure.Persistence;
using TallerMecanicoFinal.Infrastructure.Repositories;

namespace TallerMecanicoFinal.Tests.Reports;

public class WorkshopReportingServiceTests
{
    [Fact]
    public async Task SearchHistoryAsync_ReturnsOnlyCompletedAndFutureProgrammedAppointments()
    {
        using var context = CreateContext();
        var mechanic = new Mechanic(Guid.NewGuid(), "Laura Gómez", "9002");

        var completed = ServiceAppointment.Schedule("ABC123", "Toyota", "Corolla", "111", "Ana", 10000, "555", DateTime.Now.AddDays(1));
        completed.AssignMechanic(mechanic, DateTime.Now);
        completed.Complete("Diagnóstico", "Solución", DateTime.Now);

        var futureProgrammed = ServiceAppointment.Schedule("ABC123", "Nissan", "Versa", "111", "Ana", 12000, "555", DateTime.Now.AddDays(2));

        var inProcess = ServiceAppointment.Schedule("ABC123", "Kia", "Rio", "111", "Ana", 13000, "555", DateTime.Now.AddDays(3));
        inProcess.AssignMechanic(mechanic, DateTime.Now);

        context.Mechanics.Add(mechanic);
        context.Appointments.AddRange(completed, futureProgrammed, inProcess);
        await context.SaveChangesAsync();

        var service = new WorkshopReportingService(new UnitOfWork(context));
        var model = await service.SearchHistoryAsync("abc123", DateTime.Now);

        Assert.True(model.HasSearch);
        Assert.Equal(2, model.Results.Count);
        Assert.All(model.Results, item => Assert.Equal("ABC123", item.Plate));
        Assert.Contains(model.Results, item => item.StatusLabel == "Completado");
        Assert.Contains(model.Results, item => item.StatusLabel == "Programada");
    }

    [Fact]
    public async Task GetMechanicProfileAsync_ReturnsWeeklyCompletedCount()
    {
        using var context = CreateContext();
        var mechanic = new Mechanic(Guid.NewGuid(), "Laura Gómez", "9002");
        var weekStart = GetStartOfWeek(DateTime.Now);

        var completedThisWeekOne = ServiceAppointment.Schedule("AAA111", "Toyota", "Corolla", "111", "Ana", 10000, "555", DateTime.Now.AddDays(1));
        completedThisWeekOne.AssignMechanic(mechanic, DateTime.Now);
        completedThisWeekOne.Complete("Diag 1", "Sol 1", weekStart.AddDays(1));

        var completedThisWeekTwo = ServiceAppointment.Schedule("BBB222", "Toyota", "Yaris", "111", "Ana", 11000, "555", DateTime.Now.AddDays(2));
        completedThisWeekTwo.AssignMechanic(mechanic, DateTime.Now);
        completedThisWeekTwo.Complete("Diag 2", "Sol 2", weekStart.AddDays(2));

        var completedLastWeek = ServiceAppointment.Schedule("CCC333", "Toyota", "Hilux", "111", "Ana", 12000, "555", DateTime.Now.AddDays(3));
        completedLastWeek.AssignMechanic(mechanic, DateTime.Now);
        completedLastWeek.Complete("Diag 3", "Sol 3", weekStart.AddDays(-1));

        context.Mechanics.Add(mechanic);
        context.Appointments.AddRange(completedThisWeekOne, completedThisWeekTwo, completedLastWeek);
        await context.SaveChangesAsync();

        var service = new WorkshopReportingService(new UnitOfWork(context));
        var profile = await service.GetMechanicProfileAsync(mechanic.Id, DateTime.Now);

        Assert.NotNull(profile);
        Assert.Equal(2, profile!.WeeklyCompletedCount);
        Assert.Equal(mechanic.FullName, profile.FullName);
    }

    [Fact]
    public async Task GetMonthlyPerformanceAsync_ReturnsCompletedCountsPerMechanic()
    {
        using var context = CreateContext();
        var mechanicOne = new Mechanic(Guid.NewGuid(), "Laura Gómez", "9002");
        var mechanicTwo = new Mechanic(Guid.NewGuid(), "Carlos Ruiz", "9003");
        var monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        var oneA = ServiceAppointment.Schedule("AAA111", "Toyota", "Corolla", "111", "Ana", 10000, "555", DateTime.Now.AddDays(1));
        oneA.AssignMechanic(mechanicOne, DateTime.Now);
        oneA.Complete("Diag 1", "Sol 1", monthStart.AddDays(1));

        var oneB = ServiceAppointment.Schedule("BBB222", "Toyota", "Yaris", "111", "Ana", 11000, "555", DateTime.Now.AddDays(2));
        oneB.AssignMechanic(mechanicOne, DateTime.Now);
        oneB.Complete("Diag 2", "Sol 2", monthStart.AddDays(2));

        var twoA = ServiceAppointment.Schedule("CCC333", "Nissan", "Versa", "111", "Ana", 12000, "555", DateTime.Now.AddDays(3));
        twoA.AssignMechanic(mechanicTwo, DateTime.Now);
        twoA.Complete("Diag 3", "Sol 3", monthStart.AddDays(3));

        var previousMonth = ServiceAppointment.Schedule("DDD444", "Kia", "Rio", "111", "Ana", 13000, "555", DateTime.Now.AddDays(4));
        previousMonth.AssignMechanic(mechanicTwo, DateTime.Now);
        previousMonth.Complete("Diag 4", "Sol 4", monthStart.AddMonths(-1).AddDays(1));

        context.Mechanics.AddRange(mechanicOne, mechanicTwo);
        context.Appointments.AddRange(oneA, oneB, twoA, previousMonth);
        await context.SaveChangesAsync();

        var service = new WorkshopReportingService(new UnitOfWork(context));
        var report = await service.GetMonthlyPerformanceAsync(DateTime.Now);

        var mechanicOneRow = report.Rows.Single(row => row.MechanicId == mechanicOne.Id);
        var mechanicTwoRow = report.Rows.Single(row => row.MechanicId == mechanicTwo.Id);

        Assert.Equal(2, mechanicOneRow.CompletedCount);
        Assert.Equal(1, mechanicTwoRow.CompletedCount);
    }

    private static WorkshopDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<WorkshopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new WorkshopDbContext(options);
    }

    private static DateTime GetStartOfWeek(DateTime currentDateTime)
    {
        var date = currentDateTime.Date;
        var offset = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return date.AddDays(-offset);
    }
}