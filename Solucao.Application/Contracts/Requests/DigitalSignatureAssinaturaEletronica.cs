using System;
namespace Solucao.Application.Contracts.Requests
{
	public class DigitalSignatureAssinaturaEletronica
	{
		public bool ObrigarSignatarioInformarNome { get; set; }
		public bool ObrigarSignatarioInformarNumeroDocumento { get; set; }
		public int TipoDocumentoAInformar { get; set; }
	}
}

