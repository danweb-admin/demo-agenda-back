using Solucao.Application.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Service.Interfaces
{
    public interface IEquipamentService
    {
        Task<IEnumerable<EquipamentViewModel>> GetAll(bool ativo);

        Task<IEnumerable<EquipamentViewModel>> GetAllDistinct(bool ativo);


        Task<ValidationResult> Add(EquipamentViewModel equipament);

        Task<ValidationResult> Update(EquipamentViewModel equipament);
    }
}
