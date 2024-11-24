using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
	public class ClientEquipmentMapping : IEntityTypeConfiguration<ClientEquipment>
    {

        public void Configure(EntityTypeBuilder<ClientEquipment> builder)
        {
            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.ClientId);

            builder.Property(c => c.EquipmentRelationshipId);
        }
    }
}

