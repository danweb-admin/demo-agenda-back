using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
	public class EquipmentRelationshipEquipmentMapping : IEntityTypeConfiguration<EquipmentRelationshipEquipment>
    {
        public void Configure(EntityTypeBuilder<EquipmentRelationshipEquipment> builder)
        {
            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.EquipmentRelationshipId);

            builder.Property(c => c.EquipmentId);
        }
    }
}

