using System;
using System.Collections;
using System.Collections.Generic;

namespace Solucao.Application.Contracts.Requests
{
	public class DigitalSignatureAssinarOnline
	{
		public int AssinarComo { get; set; }
		public IList<string> PapelPessoaFisica { get; set; }
        public IList<string> PapelPessoaJuridica { get; set; }
		public int IdTipoAssinatura { get; set; }
		public DigitalSignatureAssinaturaEletronica AssinaturaEletronica { get; set; }
	}
}

