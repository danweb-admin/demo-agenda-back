using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
    public class ModelAttribrutesMapping : IEntityTypeConfiguration<ModelAttributes>
    {
        public void Configure(EntityTypeBuilder<ModelAttributes> builder)
        {
            builder.Property(c => c.Id)
               .HasColumnName("Id");

            builder.Property(c => c.FileAttribute)
                .HasColumnType("varchar(50)")
                .HasColumnName("fileAttribute")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.TechnicalAttribute)
                .HasColumnType("varchar(50)")
                .HasColumnName("technicalAttribute")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.AttributeType)
                .HasColumnType("varchar(50)")
                .HasColumnName("AttributeType")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Active)
                .HasColumnType("bit")
                .HasColumnName("active")
                .IsRequired();

            builder.Property(c => c.Order);

        }
    }
}

