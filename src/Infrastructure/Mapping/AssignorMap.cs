using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mapping;

public class AssignorMap : IEntityTypeConfiguration<AssignorEntity>
{
    public void Configure(EntityTypeBuilder<AssignorEntity> builder)
    {
        builder.ToTable("Assignors");
        builder.HasKey(k => k.Id);
        builder.HasIndex(i => i.Id);
        builder.HasIndex(i => i.Email).IsUnique();

        builder.Property(p => p.Id)
            .IsRequired()
            .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(140);

        builder.Property(p => p.Document)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(p => p.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(140);
    }
}