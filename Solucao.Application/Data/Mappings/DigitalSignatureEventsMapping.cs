using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
    public class DigitalSignatureEventsMapping : IEntityTypeConfiguration<DigitalSignatureEvents>
    {
        public void Configure(EntityTypeBuilder<DigitalSignatureEvents> builder)
        {
            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.IdProcesso)
                .IsRequired();

            builder.Property(c => c.IdConta)
                .IsRequired();

            builder.Property(c => c.IdWebhook)
                .IsRequired();

            builder.Property(c => c.Evento)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(c => c.DataHoraAtual)
                .IsRequired();
        }
    }
}

