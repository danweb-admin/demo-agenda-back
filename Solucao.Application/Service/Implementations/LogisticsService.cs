using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Solucao.Application.Contracts;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Service.Interfaces;
using Solucao.Application.Utils.Enum;

namespace Solucao.Application.Service.Implementations
{
    public class LogisticsService : ILogisticsService
    {
        private readonly LogisticsRepository repository;
        private readonly IMapper mapper;
        private readonly HistoryRepository history;
            private readonly CalendarRepository calendarRepository;


        public LogisticsService(
            LogisticsRepository repository,
            IMapper mapper,
            HistoryRepository history,
            CalendarRepository calendarRepository)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.history = history;
            this.calendarRepository = calendarRepository;
        }

        public async Task<IEnumerable<LogisticsViewModel>> GetAll()
        {
            return mapper.Map<IEnumerable<LogisticsViewModel>>(await repository.GetAll());
        }

        public async Task<IEnumerable<LogisticsViewModel>> GetByDate(DateTime data)
        {
            var eventos = await repository.GetByDate(data);

            var result = mapper.Map<List<LogisticsViewModel>>(eventos);

            foreach (var item in result)
            {
                var entity = eventos.First(x => x.Id == item.Id);

                item.Cliente = entity.Calendar?.Client?.Name;
                item.Equipamento = entity.Calendar?.Equipament?.Name;
                item.Cidade = entity.Calendar?.Client.City?.Nome;
                item.Motorista = entity.Driver?.Name;
                item.Placa = entity.Driver?.Plate;
                
            }

            return result;
        }

        public async Task<IEnumerable<LogisticsViewModel>> GetByDriver(Guid driverId, DateTime data)
        {
            return mapper.Map<IEnumerable<LogisticsViewModel>>(await repository.GetByDriver(driverId, data));
        }

        public async Task<LogisticsViewModel> GetById(Guid id)
        {
            return mapper.Map<LogisticsViewModel>(await repository.GetById(id));
        }

        public async Task<ValidationResult> Add(LogisticsViewModel logistics, Guid loggedUserId)
        {
            logistics.Id = Guid.NewGuid();
            logistics.CreatedAt = DateTime.Now;
            logistics.Active = true;

            var entity = mapper.Map<Logistics>(logistics);

            var result = await repository.Add(entity);

            if (result != null)
                return new ValidationResult("Houve um problema ao salvar o evento logístico.");

            await history.Add(
                TableEnum.Logistics,
                logistics.Id.Value,
                OperationEnum.Criacao,
                loggedUserId);

            return ValidationResult.Success;
        }

        public async Task<ValidationResult> Update(LogisticsViewModel logistics, Guid loggedUserId)
        {
            logistics.UpdatedAt = DateTime.Now;

            var entity = mapper.Map<Logistics>(logistics);

            var result = await repository.Update(entity);

            if (result == null)
                return new ValidationResult("Houve um problema ao atualizar o evento logístico.");

            await history.Add(
                TableEnum.Logistics,
                logistics.Id.Value,
                OperationEnum.Alteracao,
                loggedUserId);

            return ValidationResult.Success;
        }

        public async Task<ValidationResult> Remove(Guid id, Guid loggedUserId)
        {
            var result = await repository.Remove(id);

            if (result == null)
                return new ValidationResult("Houve um problema ao remover o evento logístico.");

            await history.Add(
                TableEnum.Logistics,
                id,
                OperationEnum.Exclusao,
                loggedUserId);

            return ValidationResult.Success;
        }

        public async Task<ValidationResult> Complete(Guid id, Guid loggedUserId)
        {
            var result = await repository.Complete(id);

            if (result == null)
                return new ValidationResult("Evento não encontrado.");

            await history.Add(
                TableEnum.Logistics,
                id,
                OperationEnum.Alteracao,
                loggedUserId);

            return ValidationResult.Success;
        }

        public async Task<ValidationResult> Uncomplete(Guid id, Guid loggedUserId)
        {
            var result = await repository.Uncomplete(id);

            if (result == null)
                return new ValidationResult("Evento não encontrado.");

            await history.Add(
                TableEnum.Logistics,
                id,
                OperationEnum.Alteracao,
                loggedUserId);

            return ValidationResult.Success;
        }

        public async Task SincronizarLocacao(Guid calendarId)
        {
          var calendar = await calendarRepository.GetById(calendarId);

          if (calendar == null)
              return;

          // Remove eventos existentes
          await repository.RemoveByCalendar(calendarId);

          // Entrega
          if (calendar.DriverId != null)
          {
              await repository.Add(new Logistics
              {
                  Id = Guid.NewGuid(),
                  CalendarId = calendar.Id,
                  DriverId = calendar.DriverId,
                  DataHora = calendar.StartTime.Value,
                  Tipo = TipoEventoLogistico.Entrega,
                  Titulo = "Entrega",
                  Endereco = calendar.Client.Address,
                  Active = true,
                  CreatedAt = DateTime.Now
              });
          }

          // Recolha
          if (calendar.DriverCollectsId != null)
          {
              await repository.Add(new Logistics
              {
                  Id = Guid.NewGuid(),
                  CalendarId = calendar.Id,
                  DriverId = calendar.DriverCollectsId,
                  DataHora = calendar.EndTime.Value,
                  Tipo = TipoEventoLogistico.Recolha,
                  Titulo = "Recolha",
                  Endereco = calendar.Client.Address,
                  Active = true,
                  CreatedAt = DateTime.Now
              });
          }
        }

        public async Task RemoverLocacao(Guid calendarId)
        {
            await repository.RemoveByCalendar(calendarId);
        }
    }
}