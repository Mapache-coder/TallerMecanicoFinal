using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TallerMecanicoFinal.Domain.Entities;
using TallerMecanicoFinal.Domain.Enums;

namespace TallerMecanicoFinal.Infrastructure.Persistence.Configurations;

public sealed class ServiceAppointmentConfiguration : IEntityTypeConfiguration<ServiceAppointment>
{
    public void Configure(EntityTypeBuilder<ServiceAppointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Plate)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Brand)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Model)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.OwnerDocument)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.OwnerName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.OwnerPhone)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Mileage)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ScheduledAt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Diagnosis)
            .HasMaxLength(2000);

        builder.Property(x => x.Solution)
            .HasMaxLength(2000);

        builder.HasOne(x => x.AssignedMechanic)
            .WithMany(x => x.AssignedAppointments)
            .HasForeignKey(x => x.AssignedMechanicId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ScheduledAt)
            .IsUnique();
    }
}