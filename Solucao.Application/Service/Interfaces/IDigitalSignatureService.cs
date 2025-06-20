using System;
using Solucao.Application.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Solucao.Application.Service.Interfaces
{
	public interface IDigitalSignatureService
	{
        Task<ValidationResult> EnviarDocumentoParaAssinar(Guid CalendarId);

    }
}

