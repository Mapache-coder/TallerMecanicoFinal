using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TallerMecanicoFinal.Domain.Entities;

namespace TallerMecanicoFinal.Infrastructure.Persistence.Configurations;

public sealed class WorkshopRoleConfiguration : IEntityTypeConfiguration<WorkshopRole>
{
    public void Configure(EntityTypeBuilder<WorkshopRole> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}