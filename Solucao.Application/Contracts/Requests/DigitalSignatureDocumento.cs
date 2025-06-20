using System;
namespace Solucao.Application.Contracts.Requests
{
	public class DigitalSignatureDocumento
	{
		public int OrdemDocumento { get; set; }
		public string NomeComExtensao { get; set; }
		public string Arquivo { get; set; }
	}
}

