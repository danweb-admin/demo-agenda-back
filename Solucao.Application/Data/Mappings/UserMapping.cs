using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(c => c.Id)
                .HasColumnName("Id");

            builder.Property(c => c.Name)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.NickName)
                .HasColumnType("varchar(4)")
                .HasMaxLength(4)
                .IsRequired();

            builder.Property(c => c.Password)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Role)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.Email)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("datetime");

            builder.Property(c => c.Token_Expire)
                .HasColumnName("token_expire")
                .HasColumnType("datetime2");

            builder.Property(c => c.Token)
                .HasColumnName("token")
                .HasColumnType("varchar(100)")
                .HasMaxLength(100);


            builder.Property(c => c.Active)
                .HasColumnType("bit")
                .IsRequired();
        }
    }
}
