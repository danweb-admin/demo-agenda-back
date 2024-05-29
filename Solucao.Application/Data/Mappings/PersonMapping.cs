using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
    public class PersonMapping : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.Name)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Plate)
                .HasColumnType("varchar(8)")
                .HasMaxLength(8)
                .IsRequired();

            builder.Property(c => c.CellPhone)
                .HasColumnType("varchar(15)")
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(c => c.PersonType)
                .HasColumnType("char(1)")
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("datetime");


            builder.Property(c => c.Active)
                .HasColumnType("bit")
                .IsRequired();

            builder.Property(c => c.UserId);
        }
    }
}
