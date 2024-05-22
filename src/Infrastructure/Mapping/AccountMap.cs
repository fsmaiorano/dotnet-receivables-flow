using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mapping;

public class AccountMap : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(k => k.Id);
        builder.HasIndex(i => i.Id);
        builder.HasIndex(i => i.Email).IsUnique();

        builder.Property(p => p.Id)
            .IsRequired()
            .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(140);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(140);

        builder.Property(p => p.PasswordHash)
            .IsRequired()
            .HasMaxLength(140);

        builder.Property(p => p.Role)
            .IsRequired();
    }
}