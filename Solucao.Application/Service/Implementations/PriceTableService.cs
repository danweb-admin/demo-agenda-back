using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Service.Interfaces;

namespace Solucao.Application.Service.Implementations
{
    public class PriceTableService : IPriceTableService
    {
        private PriceTableRepository repository;
        private readonly IMapper mapper;

        public PriceTableService(PriceTableRepository _repository, IMapper _mapper)
        {
            repository = _repository;
            mapper = _mapper;
        }


        public Task<ValidationResult> Add(PriceTableViewModel priceTable)
        {
            priceTable.Id = Guid.NewGuid();
            priceTable.CreatedAt = DateTime.Now;
            var _priceTable = mapper.Map<PriceTable>(priceTable);
            return repository.Add(_priceTable);
        }

        public async Task<IEnumerable<PriceTableViewModel>> GetAll(Guid equipmentId)
        {
            return mapper.Map<IEnumerable<PriceTableViewModel>>(await repository.GetAll(equipmentId));

        }

        public Task<ValidationResult> Update(PriceTableViewModel priceTable)
        {
            priceTable.UpdatedAt = DateTime.Now;
            var _priceTable = mapper.Map<PriceTable>(priceTable);

            return repository.Update(_priceTable);
        }

        public async Task<ValidationResult> Save(List<PriceTableValuesRequest> valores)
        {
            if (valores.Count == 0)
                throw new Exception("Lista vazia.");

            foreach (var equipamento in valores)
            {
                // Busca os registros atuais do banco
                var existentes = await repository.GetAll(equipamento.Id);

                var novos = new List<PriceTableViewModel>();

                foreach (var valor in equipamento.Valores)
                {
                    if (valor.Id == null || valor.Id == Guid.Empty)
                    {
                        // Novo registro
                        novos.Add(new PriceTableViewModel
                        {
                            Id = Guid.NewGuid(),
                            EquipmentId = equipamento.Id,
                            Minutes = valor.Minutes,
                            Value = valor.Value,
                            Active = true,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        // Atualização de registro existente
                        var existente = existentes.FirstOrDefault(x => x.Id == valor.Id);
                        if (existente != null)
                        {
                            existente.Minutes = valor.Minutes;
                            existente.Value = valor.Value;
                            existente.UpdatedAt = DateTime.UtcNow;
                            await repository.Update(existente);
                        }
                    }
                }

                // Identifica registros que foram removidos no frontend
                var idsRecebidos = equipamento.Valores
                    .Where(v => v.Id != null)
                    .Select(v => v.Id.Value)
                    .ToList();

                var removidos = existentes
                    .Where(e => !idsRecebidos.Contains(e.Id))
                    .ToList();

                // Marca como inativos (ou remove fisicamente)
                foreach (var removido in removidos)
                {
                    removido.Active = false;
                    removido.UpdatedAt = DateTime.UtcNow;
                    await repository.Update(removido);
                }

                // Adiciona novos registros
                if (novos.Any())
                    await repository.AddRange(mapper.Map<List<PriceTable>>(novos));

                await repository.SaveChange();
            }

            return ValidationResult.Success;
        }

        public async Task<decimal> ValueByEquipment(PriceTableRequest model)
        {
            model.StartTime = model.StartTime.Replace(":", "");
            model.EndTime = model.EndTime.Replace(":", "");
            var now = DateTime.Now;

            var _startTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(model.StartTime.Substring(0, 2)), int.Parse(model.StartTime.Substring(2)), 0);
            var _endTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(model.EndTime.Substring(0, 2)), int.Parse(model.EndTime.Substring(2)), 0);

            // Calcula tempo total em minutos
            int totalMinutos = (int)Math.Ceiling((_endTime - _startTime).TotalMinutes);

            // Busca todas as faixas do aparelho
            var faixas = await repository.GetAll(model.EquipmentId);

            faixas = faixas.OrderBy(p => p.Minutes).ToList();

            if (!faixas.Any())
                  throw new Exception("Não existe tabela de preços para o aparelho informado.");

            // Encontra a primeira faixa com tempo >= tempo solicitado
            var faixa = faixas.FirstOrDefault(p => p.Minutes >= totalMinutos);

            // Se não houver faixa suficiente, pega a última disponível (tempo máximo)
            if (faixa == null)
                faixa = faixas.Last();

            return faixa.Value;

        }
    }
}

