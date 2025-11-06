using System;
using Solucao.Application.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Solucao.Application.Contracts.Requests;

namespace Solucao.Application.Service.Interfaces
{
	public interface IPriceTableService
	{
        Task<IEnumerable<PriceTableViewModel>> GetAll(Guid equipmentId);

        Task<ValidationResult> Add(PriceTableViewModel priceTable);

        Task<ValidationResult> Save(List<PriceTableValuesRequest> valores);


        Task<ValidationResult> Update(PriceTableViewModel priceTable);

        Task<decimal> ValueByEquipment(PriceTableRequest model);
    }
}

