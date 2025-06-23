using System;
using Solucao.Application.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Solucao.Application.Contracts.Response;

namespace Solucao.Application.Service.Interfaces
{
	public interface IDigitalSignatureService
	{
        Task<ValidationResult> EnviarDocumentoParaAssinar(Guid CalendarId);
        Task<ValidationResult> EventosWebhook(DigitalSignatureResponse response);

    }
}

