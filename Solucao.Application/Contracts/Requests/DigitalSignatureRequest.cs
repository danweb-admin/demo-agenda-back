using System;
using System.Collections.Generic;

namespace Solucao.Application.Contracts.Requests
{
	public class DigitalSignatureRequest
	{
        public Guid IdPasta { get; set; }
        public Guid IdResponsavel { get; set; }
        public string NomeProcesso { get; set; }
        public DigitalSignatureMensagemPadrao MensagemPadrao { get; set; }
        public bool UsarOrdemAssinatura { get; set; }
        public bool UsarPosicaoAssinaturaAutomatica { get; set; }
        public IList<DigitalSignatureDestinatario> Destinatarios { get; set; }
        public IList<DigitalSignatureDocumento> Documentos { get; set; }

    }
}

