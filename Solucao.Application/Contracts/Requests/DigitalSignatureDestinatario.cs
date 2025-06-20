using System;
namespace Solucao.Application.Contracts.Requests
{
	public class DigitalSignatureDestinatario
	{
		public int IdTipoAcao { get; set; }
		public int OrdemAssinatura { get; set; }
		public string Nome { get; set; }
		public string Email { get; set; }
		public string Telefone { get; set; }
		public DigitalSignatureAssinarOnline AssinarOnline { get; set; }
	}
}

