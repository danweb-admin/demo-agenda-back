using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Solucao.Application.Contracts;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Service.Interfaces;

namespace Solucao.Application.Service.Implementations
{
	public class ModelAttributesService : IModelAttributesService
	{
        private readonly ModelAttributesRepository repository;
        private readonly IMapper mapper;

        public ModelAttributesService(ModelAttributesRepository _repository, IMapper _mapper)
		{
            repository = _repository;
            mapper = _mapper;
		}

        public async Task<ValidationResult> Add(ModelAttributeViewModel model)
        {
            model.Id = Guid.NewGuid();
            model.CreatedAt = DateTime.Now;
            var _model = mapper.Map<ModelAttributes>(model);
            return await repository.Add(_model);
        }

        public async Task<IEnumerable<ModelAttributeViewModel>> GetAll(Guid modelId)
        {
            return mapper.Map<IEnumerable<ModelAttributeViewModel>>(await repository.GetAll());
        }

        public async Task<ValidationResult> Remove(Guid id)
        {
            return await repository.Remove(id);
        }

        public async Task<ValidationResult> Update(ModelAttributeViewModel model)
        {
            var _model = mapper.Map<ModelAttributes>(model);

            return await repository.Update(_model);
        }
    }
}

