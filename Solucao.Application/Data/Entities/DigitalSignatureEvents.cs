using System;
namespace Solucao.Application.Data.Entities
{
	public class DigitalSignatureEvents
	{
		public Guid Id { get; set; }
		public Guid IdProcesso { get; set; }
		public string Evento { get; set; }
		public Guid IdConta { get; set; }
		public Guid IdWebhook { get; set; }
		public DateTime DataHoraAtual { get; set; }

	}
}

