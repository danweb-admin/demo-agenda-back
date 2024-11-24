using System;
using Solucao.Application.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Solucao.Application.Service.Interfaces
{
	public interface IEquipmentRelantionshipService
	{
        Task<IEnumerable<EquipmentRelationshipViewModel>> GetAll(bool ativo);

        Task<ValidationResult> Add(EquipmentRelationshipViewModel equipmentRelationship);

        Task<ValidationResult> Update(EquipmentRelationshipViewModel equipmentRelationship);
    }
}

