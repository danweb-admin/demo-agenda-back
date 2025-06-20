using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
    public class DigitalSignatureMapping : IEntityTypeConfiguration<DigitalSignature>
    {
        public void Configure(EntityTypeBuilder<DigitalSignature> builder)
        {
            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.CreatedAt)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("datetime");

            builder.Property(c => c.Active)
                .HasColumnType("bit")
                .IsRequired();

            builder.Property(c => c.IdProcesso);

            builder.Property(c => c.IdResponsavel)
                .IsRequired();

            builder.Property(c => c.IdPasta)
               .IsRequired();

            builder.Property(c => c.CalendarId)
               .IsRequired();

            builder.Property(c => c.NomeProcesso)
               .HasMaxLength(150)
               .IsRequired();

            builder.Property(c => c.Status)
               .HasMaxLength(15)
               .IsRequired();
        }
    }
}

