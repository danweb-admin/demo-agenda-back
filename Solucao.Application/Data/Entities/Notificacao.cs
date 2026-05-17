using System;
namespace Solucao.Application.Data.Entities
{
    public class Notificacao : BaseEntity
    {
        public Guid LocacaoId { get; set; }

        public string Telefone { get; set; }
        public string Mensagem { get; set; }


        /// <summary>
        /// P - Pendente
        /// E - Enviado
        /// F - Falha
        /// R - Respondido
        /// </summary>
        public char Status { get; set; }

        public DateTime? DataEnvio { get; set; }
        public DateTime? DataResposta { get; set; }

        /// <summary>
        /// Confirmado / Cancelado
        /// </summary>
        public string Resposta { get; set; }

        public string TokenConfirmacao { get; set; }

        public int Tentativas { get; set; }
        public Calendar Locacao { get; set; }
  }
}

