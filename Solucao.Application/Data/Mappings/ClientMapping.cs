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
    public class ClientMapping : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.Name)
                .HasColumnType("varchar(200)")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.CellPhone)
                .HasColumnType("varchar(15)")
                .HasMaxLength(10);

            builder.Property(c => c.CreatedAt)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("datetime");


            builder.Property(c => c.Active)
                .HasColumnType("bit")
                .IsRequired();

            builder.Property(c => c.Phone)
                .HasColumnType("varchar(14)")
                .HasMaxLength(14);

            builder.Property(c => c.Email)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50);

            builder.Property(c => c.Address)
                .HasColumnType("varchar(70)")
                .HasMaxLength(70)
                .IsRequired();

            builder.Property(c => c.Number)
                .HasColumnType("varchar(15)")
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(c => c.Complement)
                .HasColumnType("varchar(50)")
                .HasMaxLength(30);

            builder.Property(c => c.CityId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.StateId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.Neighborhood)
                .HasColumnType("varchar(50)")
                .HasMaxLength(30);

            builder.Property(c => c.IsPhysicalPerson)
                .HasColumnType("bit");

            builder.Property(c => c.IsAnnualContract)
                .HasColumnType("bit");


            builder.Property(c => c.IsReceipt)
                .HasColumnType("int");

            builder.Property(c => c.NameForReceipt)
                .HasColumnType("varchar(100)")
                .HasMaxLength(100);

            builder.Property(c => c.HasAirConditioning)
                .HasColumnType("bit");

            builder.Property(c => c.Has220V)
                .HasColumnType("bit");

            builder.Property(c => c.HasStairs)
                .HasColumnType("bit");

            builder.Property(c => c.TakeTransformer)
                .HasColumnType("bit");

            builder.Property(c => c.HasTechnique)
                .HasColumnType("bit");

            builder.Property(c => c.TechniqueOption1)
                .HasColumnType("varchar(30)")
                .HasMaxLength(30);

            builder.Property(c => c.TechniqueOption2)
                .HasColumnType("varchar(30)")
                .HasMaxLength(30);

            builder.Property(c => c.LandMark)
                .HasColumnType("varchar(100)")
                .HasMaxLength(100);

            builder.Property(c => c.Responsible)
                .HasColumnType("varchar(100)")
                .HasMaxLength(70);

            builder.Property(c => c.Specialty)
                .HasColumnType("varchar(200)")
                .HasMaxLength(200);

            builder.Property(c => c.ClinicCellPhone)
                .HasColumnType("varchar(15)")
                .HasMaxLength(15);

            builder.Property(c => c.ClinicName)
                .HasColumnType("varchar(200)")
                .HasMaxLength(50);

            builder.Property(c => c.ZipCode)
                .HasColumnType("varchar(9)")
                .HasMaxLength(9);

            builder.Property(c => c.Secretary)
                .HasColumnType("varchar(30)")
                .HasMaxLength(30);

            builder.Property(c => c.Cpf)
                .HasColumnType("varchar(14)")
                .HasMaxLength(14);

            builder.Property(c => c.Cnpj)
                .HasColumnType("varchar(18)")
                .HasMaxLength(18);

            builder.Property(c => c.Rg)
                .HasColumnType("varchar(20)")
                .HasMaxLength(20);

            builder.Property(c => c.Ie)
                .HasColumnType("varchar(20)")
                .HasMaxLength(20);

            builder.Property(c => c.Discount)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.Freight)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.EquipamentValues)
                .HasColumnType("varchar(1500)")
                .HasMaxLength(1500);

        }
    }
}
