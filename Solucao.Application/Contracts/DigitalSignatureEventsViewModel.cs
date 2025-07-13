using System;
namespace Solucao.Application.Contracts
{
	public class DigitalSignatureEventsViewModel
	{
        public Guid Id { get; set; }
        public Guid IdProcesso { get; set; }
        public string Evento { get; set; }
        public Guid IdConta { get; set; }
        public Guid IdWebhook { get; set; }
        public DateTime DataHoraAtual { get; set; }
    }
}

