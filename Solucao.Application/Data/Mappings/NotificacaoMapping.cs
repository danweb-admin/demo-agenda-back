using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Mappings
{
  public class NotificacaoMappings : IEntityTypeConfiguration<Notificacao>
  {
      public void Configure(EntityTypeBuilder<Notificacao> builder)
      {
          
          // 🔑 Primary Key
          builder.HasKey(n => n.Id);

          builder.Property(n => n.Id)
                  .IsRequired();

          builder.Property(n => n.LocacaoId)
                  .IsRequired();

          // 📞 Telefone
          builder.Property(n => n.Telefone)
                  .IsRequired()
                  .HasMaxLength(20);

          // 📝 Mensagem
          builder.Property(n => n.Mensagem)
                  .IsRequired()
                  .HasColumnType("NVARCHAR(MAX)");

          // 🔄 Active
          builder.Property(n => n.Active)
                  .HasDefaultValue(true);

          // 📊 Status (char)
          builder.Property(n => n.Status)
                  .IsRequired()
                  .HasColumnType("CHAR(1)");

          // 📅 Datas
          builder.Property(n => n.CreatedAt)
                  .IsRequired()
                  .HasDefaultValueSql("GETDATE()");

          builder.Property(n => n.UpdatedAt);

          builder.Property(n => n.DataEnvio);

          builder.Property(n => n.DataResposta);

          // ✅ Resposta
          builder.Property(n => n.Resposta)
                  .HasMaxLength(20);

          // 🔐 Token
          builder.Property(n => n.TokenConfirmacao)
                  .HasMaxLength(100);

          // 🔁 Tentativas
          builder.Property(n => n.Tentativas)
                  .IsRequired()
                  .HasDefaultValue(0);


          // 📌 Índices (recomendado)
          builder.HasIndex(n => n.LocacaoId);
          builder.HasIndex(n => n.Status);
          builder.HasIndex(n => n.TokenConfirmacao);

          
      }
  }
}

