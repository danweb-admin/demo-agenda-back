using System;
using Solucao.Application.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Solucao.Application.Contracts.Requests;

namespace Solucao.Application.Service.Interfaces
{
	public interface IGenerateContractService
	{
        Task<IEnumerable<CalendarViewModel>> GetAllByDayAndContractMade(DateTime date);
        Task<ValidationResult> GenerateContract(GenerateContractRequest request, IEnumerable<CalendarViewModel> listaLocacoes);
        Task<ValidationResult> GenerateMultipleContract(string ids);
        Task<IEnumerable<CalendarViewModel>> BuscarLocacoes( Guid cliendId, Guid equipmentId, DateTime startDate, DateTime endDate);
        Task<Byte[]> DownloadContract(Guid calendarId);

    }
}

