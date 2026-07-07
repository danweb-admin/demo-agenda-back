using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Solucao.Application.Contracts;

namespace Solucao.Application.Service.Interfaces
{
    public interface ILogisticsService
    {
        Task<IEnumerable<LogisticsViewModel>> GetAll();

        Task<IEnumerable<LogisticsViewModel>> GetByDate(DateTime data);

        Task<IEnumerable<LogisticsViewModel>> GetByDriver(Guid driverId, DateTime data);

        Task<LogisticsViewModel> GetById(Guid id);

        Task<ValidationResult> Add(LogisticsViewModel logistics, Guid loggedUserId);

        Task<ValidationResult> Update(LogisticsViewModel logistics, Guid loggedUserId);

        Task<ValidationResult> Remove(Guid id, Guid loggedUserId);

        Task<ValidationResult> Complete(Guid id, Guid loggedUserId);

        Task<ValidationResult> Uncomplete(Guid id, Guid loggedUserId);

        Task SincronizarLocacao(Guid calendarId);

        Task RemoverLocacao(Guid calendarId);
    }
}