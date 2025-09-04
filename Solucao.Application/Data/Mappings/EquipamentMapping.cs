using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Data.Mappings
{
    public class EquipamentMapping : IEntityTypeConfiguration<Equipament>
    {
        public void Configure(EntityTypeBuilder<Equipament> builder)
        {
            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.Name)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("datetime");

            builder.Property(c => c.Active)
                .HasColumnType("bit")
                .IsRequired();

            builder.Property(c => c.Order)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.Color)
                .HasColumnType("varchar(7)");


        }
    }
}
