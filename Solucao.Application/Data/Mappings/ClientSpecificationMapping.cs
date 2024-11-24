using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
    public class ClientSpecificationMapping : IEntityTypeConfiguration<ClientSpecification>
    {
        public void Configure(EntityTypeBuilder<ClientSpecification> builder)
        {
            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.ClientId);

            builder.Property(c => c.SpecificationId);

            builder.Property(c => c.Hours);

            builder.Property(c => c.Value)
                .HasColumnType("decimal(18,2)");

        }
    }
}

