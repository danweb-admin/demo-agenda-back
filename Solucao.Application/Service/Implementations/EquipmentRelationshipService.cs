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
	public class EquipmentRelationshipService : IEquipmentRelantionshipService
	{
        private EquipmentRelationshipRepository repository;
        private readonly IMapper mapper;

        public EquipmentRelationshipService(EquipmentRelationshipRepository _repository, IMapper _mapper)
		{
            repository = _repository;
            mapper = _mapper;
        }

        public async Task<ValidationResult> Add(EquipmentRelationshipViewModel equipmentRelationship)
        {
            equipmentRelationship.Id = Guid.NewGuid();
            equipmentRelationship.CreatedAt = DateTime.Now;
            equipmentRelationship.Active = true;
            var _equipament = mapper.Map<EquipmentRelationship>(equipmentRelationship);
            return await repository.Add(_equipament);
        }

        public async Task<IEnumerable<EquipmentRelationshipViewModel>> GetAll(bool ativo)
        {
            return mapper.Map<IEnumerable<EquipmentRelationshipViewModel>>(await repository.GetAll(ativo));
        }

        public async Task<ValidationResult> Update(EquipmentRelationshipViewModel equipmentRelationship)
        {
            var _equipament = mapper.Map<EquipmentRelationship>(equipmentRelationship);

            _equipament.UpdatedAt = DateTime.Now;

            return await repository.Update(_equipament);
        }
    }
}

