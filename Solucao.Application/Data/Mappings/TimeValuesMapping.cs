using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
	public class TimeValuesMapping : IEntityTypeConfiguration<TimeValue>
    {

        public void Configure(EntityTypeBuilder<TimeValue> builder)
        {
            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.Time)
                .HasColumnType("char(5)")
                .IsRequired();

            builder.Property(c => c.Value)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(c => c.ClientEquipmentId)
                .IsRequired();
        }
    }
}

