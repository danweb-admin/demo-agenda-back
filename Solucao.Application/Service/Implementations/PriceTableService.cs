using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Interfaces;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Service.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

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

        public Task<ValidationResult> Save(List<PriceTableValuesRequest> valores)
        {
            if (valores.Count == 0)
                throw new Exception("Lista vazia.");

            foreach (var item in valores)
            {
                
            }

            return Task.Run(() => ValidationResult.Success); 
        }
    }
}

