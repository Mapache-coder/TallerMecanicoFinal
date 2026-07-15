using Moq;
using TallerMecanicoFinal.Application.Contracts;
using TallerMecanicoFinal.Application.Contracts.Repositories;
using TallerMecanicoFinal.Application.Services;
using TallerMecanicoFinal.Domain.Entities;

namespace TallerMecanicoFinal.Tests.Application;

public class AppointmentCreationServiceTests
{
    [Fact]
    public async Task CreateAsync_PersistsAppointment_And_CommitsTransaction()
    {
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var appointmentRepository = new Mock<IServiceAppointmentRepository>(MockBehavior.Strict);
        var mechanicRepository = new Mock<IMechanicRepository>(MockBehavior.Loose);
        var roleRepository = new Mock<IWorkshopRoleRepository>(MockBehavior.Loose);

        unitOfWork.SetupGet(x => x.Appointments).Returns(appointmentRepository.Object);
        unitOfWork.SetupGet(x => x.Mechanics).Returns(mechanicRepository.Object);
        unitOfWork.SetupGet(x => x.Roles).Returns(roleRepository.Object);

        appointmentRepository
            .Setup(x => x.ExistsAtAsync(It.IsAny<DateTime>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        appointmentRepository
            .Setup(x => x.AddAsync(It.IsAny<ServiceAppointment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback<ServiceAppointment, CancellationToken>((_, _) => { });

        unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var service = new AppointmentCreationService(unitOfWork.Object);
        var request = new AppointmentCreationRequest(
            "ABC123",
            "Toyota",
            "Corolla",
            "0123456789",
            "Juan Perez",
            15000,
            "555-1234",
            DateTime.Now.AddDays(1));

        var appointment = await service.CreateAsync(request);

        Assert.Equal("ABC123", appointment.Plate);
        Assert.Equal(TallerMecanicoFinal.Domain.Enums.AppointmentStatus.Registrado, appointment.Status);
        appointmentRepository.Verify(x => x.ExistsAtAsync(It.IsAny<DateTime>(), null, It.IsAny<CancellationToken>()), Times.Once);
        appointmentRepository.Verify(x => x.AddAsync(It.IsAny<ServiceAppointment>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenAppointmentAlreadyExists()
    {
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var appointmentRepository = new Mock<IServiceAppointmentRepository>(MockBehavior.Strict);
        var mechanicRepository = new Mock<IMechanicRepository>(MockBehavior.Loose);
        var roleRepository = new Mock<IWorkshopRoleRepository>(MockBehavior.Loose);

        unitOfWork.SetupGet(x => x.Appointments).Returns(appointmentRepository.Object);
        unitOfWork.SetupGet(x => x.Mechanics).Returns(mechanicRepository.Object);
        unitOfWork.SetupGet(x => x.Roles).Returns(roleRepository.Object);

        appointmentRepository
            .Setup(x => x.ExistsAtAsync(It.IsAny<DateTime>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new AppointmentCreationService(unitOfWork.Object);
        var request = new AppointmentCreationRequest(
            "ABC123",
            "Toyota",
            "Corolla",
            "0123456789",
            "Juan Perez",
            15000,
            "555-1234",
            DateTime.Now.AddDays(1));

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(request));

        appointmentRepository.Verify(x => x.AddAsync(It.IsAny<ServiceAppointment>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}