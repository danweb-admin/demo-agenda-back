using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
  public class LogisticsMappings : IEntityTypeConfiguration<Logistics>
  {
      public void Configure(EntityTypeBuilder<Logistics> builder)
      {
          builder.HasKey(x => x.Id);

          builder.Property(x => x.Id)
                 .IsRequired();

          builder.Property(x => x.DataHora)
                 .IsRequired();

          builder.Property(x => x.Tipo)
                 .IsRequired();

          builder.Property(x => x.Titulo)
                 .HasMaxLength(200);

          builder.Property(x => x.Descricao)
                 .HasColumnType("nvarchar(max)");

          builder.Property(x => x.Observacao)
                 .HasColumnType("varchar(200)");

          builder.Property(x => x.Endereco)
                 .HasMaxLength(300);

          builder.Property(x => x.Concluido)
                 .HasDefaultValue(false);

          builder.Property(x => x.Active)
                 .HasDefaultValue(true);

          builder.Property(x => x.CreatedAt)
                 .HasDefaultValueSql("GETDATE()");

          builder.Property(x => x.UpdatedAt);

          builder.HasOne(x => x.Calendar)
                 .WithMany()
                 .HasForeignKey(x => x.CalendarId)
                 .OnDelete(DeleteBehavior.Restrict);

          builder.HasOne(x => x.Driver)
                 .WithMany()
                 .HasForeignKey(x => x.DriverId)
                 .OnDelete(DeleteBehavior.Restrict);

          builder.HasIndex(x => x.DataHora);

          builder.HasIndex(x => x.DriverId);

          builder.HasIndex(x => x.Tipo);

          builder.HasIndex(x => x.CalendarId);
      }
  }
}

