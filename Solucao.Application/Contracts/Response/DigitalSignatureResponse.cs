using System;
using System.Collections.Generic;

namespace Solucao.Application.Contracts.Response
{
	public class DigitalSignatureResponse
	{
		public Guid IdProcesso { get; set; }
        public string Evento { get; set; }
        public int IdEvento { get; set; }
		public string DataHoraAtual { get; set; }

        public ICollection<DigitalSignatureSignatarioResponse> Signatarios { get; set; }
        public ICollection<DigitalSignatureDestinatarioResponse> Destinatarios { get; set; }

    }
}