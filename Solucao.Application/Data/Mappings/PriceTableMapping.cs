using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
    public class PriceTableMapping : IEntityTypeConfiguration<PriceTable>
    {
        public void Configure(EntityTypeBuilder<PriceTable> builder)
        {
            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.EquipmentId)
                .IsRequired();

            builder.Property(c => c.Value)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(c => c.Minutes)
                .HasColumnType("int")
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("datetime");


            builder.Property(c => c.Active)
                .HasColumnType("bit")
                .IsRequired();
        }
    }
}

