using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Solucao.Application.Contracts;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Interfaces;
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
        private readonly IPersonRepository personRepository;


        public LogisticsService(
            LogisticsRepository repository,
            IMapper mapper,
            HistoryRepository history,
            CalendarRepository calendarRepository,
            IPersonRepository personRepository)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.history = history;
            this.calendarRepository = calendarRepository;
            this.personRepository = personRepository;
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
            logistics.DataHoraLogistica = logistics.DataHora;
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

            var motorista = await personRepository.GetByName("M", "X");
            var motoristaPadrao = motorista.FirstOrDefault();

            if (calendar.DriverId == null && motoristaPadrao != null)
                calendar.DriverId = motoristaPadrao.Id;

            if (calendar.DriverCollectsId == null && motoristaPadrao != null)
                calendar.DriverCollectsId = motoristaPadrao.Id;

            // Busca as logísticas existentes
            var logisticas = await repository.GetByCalendarId(calendarId);

            // Entrega
            if (calendar.StartTime.HasValue && calendar.DriverId.HasValue)
            {
                var entrega = logisticas.FirstOrDefault(
                    x => x.Tipo == TipoEventoLogistico.Entrega);

                if (entrega == null)
                {
                    entrega = new Logistics
                    {
                        Id = Guid.NewGuid(),
                        CalendarId = calendar.Id,
                        Tipo = TipoEventoLogistico.Entrega,
                        CreatedAt = DateTime.Now,
                        Active = true
                    };
                }

                entrega.DriverId = calendar.DriverId.Value;
                entrega.DataHora = calendar.StartTime.Value;
                entrega.DataHoraLogistica = calendar.StartTime.Value;
                entrega.Titulo = "Entrega";
                entrega.Endereco = calendar.Client.Address;
                entrega.ObservacaoGoogle = calendar.GoogleNote;
                entrega.Active = true;

                if (!logisticas.Any(x => x.Id == entrega.Id))
                    await repository.Add(entrega);
                else
                    await repository.Update(entrega);
            }

            // Recolha
            if (calendar.EndTime.HasValue && calendar.DriverCollectsId.HasValue)
            {
                var recolha = logisticas.FirstOrDefault(
                    x => x.Tipo == TipoEventoLogistico.Recolha);

                if (recolha == null)
                {
                    recolha = new Logistics
                    {
                        Id = Guid.NewGuid(),
                        CalendarId = calendar.Id,
                        Tipo = TipoEventoLogistico.Recolha,
                        CreatedAt = DateTime.Now,
                        Active = true
                    };
                }

                recolha.DriverId = calendar.DriverCollectsId.Value;
                recolha.DataHora = calendar.EndTime.Value;
                recolha.DataHoraLogistica = calendar.EndTime.Value;
                recolha.Titulo = "Recolha";
                recolha.Endereco = calendar.Client.Address;
                recolha.ObservacaoGoogle = calendar.GoogleNote;
                recolha.Active = true;

                if (!logisticas.Any(x => x.Id == recolha.Id))
                    await repository.Add(recolha);
                else
                    await repository.Update(recolha);
            }
        }

        public async Task RemoverLocacao(Guid calendarId)
        {
            await repository.RemoveByCalendar(calendarId);
        }

        public async Task<ValidationResult> UpdateDriver(LogisticsViewModel logistics, Guid loggedUserId)
        {
            var result = await repository.GetById(logistics.Id.Value);

            if (result == null)
                return new ValidationResult("Evento não encontrado.");

            result.DriverId = logistics.DriverId;

            await repository.Update(result);
           
            if (logistics.Tipo == 3)
                return ValidationResult.Success;
          
            var calendar = await calendarRepository.GetById(logistics.CalendarId.Value);

            if (logistics.Tipo == 1)
                calendar.DriverId = logistics.DriverId;
            else
                calendar.DriverCollectsId = logistics.DriverId;

            await calendarRepository.Update(calendar);

            return ValidationResult.Success;
            
        }

        public async Task<ValidationResult> UpdateObservacao(LogisticsViewModel logistics, Guid loggedUserId)
        {
            var result = await repository.GetById(logistics.Id.Value);

            if (result == null)
                return new ValidationResult("Evento não encontrado.");

            result.Observacao = logistics.Observacao;

            await repository.Update(result);
          
            return ValidationResult.Success;
        }

        public async Task<ValidationResult> UpdateDataLog(LogisticsViewModel logistics, Guid loggedUserId)
        {
          var result = await repository.GetById(logistics.Id.Value);

            if (result == null)
                return new ValidationResult("Evento não encontrado.");

            result.DataHoraLogistica = logistics.DataHoraLogistica;

            await repository.Update(result);
          
            return ValidationResult.Success;
        }

        public async Task<ValidationResult> UpdateAcessorio(LogisticsViewModel logistics, Guid loggedUserId)
        {
          var result = await repository.GetById(logistics.Id.Value);

            if (result == null)
                return new ValidationResult("Evento não encontrado.");

            result.Acessorios = logistics.Acessorios;

            await repository.Update(result);
          
            return ValidationResult.Success;
        }
  }
}