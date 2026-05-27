using System;

namespace Solucao.Application.Contracts
{
    public class NotificacaoViewModel
    {
        public Guid? Id { get; set; }
        public Guid LocacaoId { get; set; }

        public string Telefone { get; set; }
        public string Mensagem { get; set; }

        public bool Active { get; set; }

        /// <summary>
        /// P - Pendente
        /// E - Enviado
        /// F - Falha
        /// R - Respondido
        /// </summary>
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public DateTime? DataEnvio { get; set; }
        public DateTime? DataResposta { get; set; }

        /// <summary>
        /// Confirmado / Cancelado
        /// </summary>
        public string Resposta { get; set; }

        public string TokenConfirmacao { get; set; }

        public int Tentativas { get; set; }

        public CalendarViewModel Locacao { get; set; }
  }
}