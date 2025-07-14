using System;
using System.Collections.Generic;

namespace Solucao.Application.Contracts.Response
{
	public class DigitalSignatureResponse
	{
		public Guid IdProcesso { get; set; }
        public Guid idProcesso { get; set; }
        public string Evento { get; set; }
        public string evento { get; set; }

        public int IdEvento { get; set; }
        public int idEvento { get; set; }

        
		public DateTime DataHoraAtual { get; set; }
        public DateTime dataHoraAtual { get; set; }

        public ICollection<DigitalSignatureSignatarioResponse> Signatarios { get; set; }
        public ICollection<DigitalSignatureSignatarioResponse> signatarios { get; set; }
    }
}