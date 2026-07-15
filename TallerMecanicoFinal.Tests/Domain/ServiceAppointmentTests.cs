using TallerMecanicoFinal.Domain.Entities;
using TallerMecanicoFinal.Domain.Enums;

namespace TallerMecanicoFinal.Tests.Domain;

public class ServiceAppointmentTests
{
    [Fact]
    public void Schedule_Throws_WhenScheduledAtIsInThePast()
    {
        var pastDate = DateTime.Now.AddDays(-1);

        var exception = Assert.Throws<ArgumentException>(() => ServiceAppointment.Schedule(
            plate: "ABC123",
            brand: "Toyota",
            model: "Corolla",
            ownerDocument: "0123456789",
            ownerName: "Juan Perez",
            mileage: 15000,
            ownerPhone: "555-1234",
            scheduledAt: pastDate));

        Assert.Equal("scheduledAt", exception.ParamName);
    }

    [Fact]
    public void Schedule_SetsRegisteredStatus_And_NormalizesMinutesToZero()
    {
        var scheduledAt = DateTime.Now.AddDays(1).AddMinutes(37).AddSeconds(19);

        var appointment = ServiceAppointment.Schedule(
            plate: "ABC123",
            brand: "Toyota",
            model: "Corolla",
            ownerDocument: "0123456789",
            ownerName: "Juan Perez",
            mileage: 15000,
            ownerPhone: "555-1234",
            scheduledAt: scheduledAt);

        Assert.Equal(AppointmentStatus.Registrado, appointment.Status);
        Assert.Equal(0, appointment.ScheduledAt.Minute);
        Assert.Equal(0, appointment.ScheduledAt.Second);
    }
}