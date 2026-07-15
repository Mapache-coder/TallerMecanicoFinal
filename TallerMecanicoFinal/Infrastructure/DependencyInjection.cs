using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TallerMecanicoFinal.Application.Contracts;
using TallerMecanicoFinal.Application.Contracts.Repositories;
using TallerMecanicoFinal.Application.Services;
using TallerMecanicoFinal.Infrastructure.Persistence;
using TallerMecanicoFinal.Infrastructure.Repositories;

namespace TallerMecanicoFinal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkshopPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=TallerMecanicoFinal;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        services.AddDbContext<WorkshopDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
            }));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IServiceAppointmentRepository, ServiceAppointmentRepository>();
        services.AddScoped<IMechanicRepository, MechanicRepository>();
        services.AddScoped<IWorkshopRoleRepository, WorkshopRoleRepository>();
        services.AddScoped<AppointmentCreationService>();

        return services;
    }
}