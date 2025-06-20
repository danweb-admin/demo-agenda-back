using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
    public class ClientDigitalSignatureMapping : IEntityTypeConfiguration<ClientDigitalSignature>
    {
        public void Configure(EntityTypeBuilder<ClientDigitalSignature> builder)
        {
            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.Name)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Email)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.PartyName)
                .HasColumnType("varchar(30)")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(c => c.ClientId);

            builder.Property(c => c.Active)
                .HasColumnType("bit")
                .HasColumnName("active")
                .IsRequired();

            builder.Property(c => c.IsPF)
                .HasColumnType("bit")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("datetime");
        }
    }
}

