using System;
namespace Solucao.Application.Contracts.Response
{
	public class DigitalSignatureResponse
	{
		public Guid idProcesso { get; set; }
		public string evento { get; set; }
		public int idEvento { get; set; }
		public Guid idConta { get; set; }
		public Guid idWebhook { get; set; }
		public DateTime dataHoraAtual { get; set; }
	}
}