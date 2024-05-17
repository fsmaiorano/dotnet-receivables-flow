using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mapping;

public class PayableMap : IEntityTypeConfiguration<PayableEntity>
{
    public void Configure(EntityTypeBuilder<PayableEntity> builder)
    {
        builder.ToTable("Payables");
        builder.HasKey(k => k.Id);
        builder.HasIndex(i => i.Id);

        builder.Property(p => p.Id)
            .IsRequired()
            .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

        builder.Property(p => p.AssignorId)
            .IsRequired();

        builder.Property(p => p.Value)
            .IsRequired();

        builder.Property(p => p.EmissionDate)
            .IsRequired();

        builder.HasOne(p => p.Assignor)
            .WithMany(p => p.Payables)
            .HasForeignKey(f => f.AssignorId);
    }
}