using AutoMapper;
using Solucao.Application.Contracts;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Interfaces;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Solucao.Application.Service.Implementations
{
    public class EquipamentService : IEquipamentService
    {
        private IEquipamentRepository equipamentRepository;
        private readonly IMapper mapper;

        public EquipamentService(IEquipamentRepository _equipamentRepository,IMapper _mapper)
        {
            equipamentRepository = _equipamentRepository;
            mapper = _mapper;
        }
        public async Task<ValidationResult> Add(EquipamentViewModel equipament)
        {
            equipament.Id = Guid.NewGuid();
            equipament.CreatedAt = DateTime.Now;
            var _equipament = mapper.Map<Equipament>(equipament);
            return await equipamentRepository.Add(_equipament);
        }

        public async Task<IEnumerable<EquipamentViewModel>> GetAll(bool ativo)
        {
            return mapper.Map<IEnumerable<EquipamentViewModel>>(await equipamentRepository.GetAll(ativo));
        }

        public Task<ValidationResult> Update(EquipamentViewModel equipament)
        {
            var _equipament = mapper.Map<Equipament>(equipament);

            _equipament.UpdatedAt = DateTime.Now;

            return equipamentRepository.Update(_equipament);
        }

        public async Task<IEnumerable<EquipamentViewModel>> GetAllDistinct(bool ativo)
        {
            var equipament = mapper.Map<IEnumerable<EquipamentViewModel>>(await equipamentRepository.GetAll(ativo));
            var retorno = new List<EquipamentViewModel>();
            string pattern = @"[^a-zA-Z\s]";

            foreach (var item in equipament)
            {
                string result = Regex.Replace(item.Name, pattern, string.Empty);
                if (!retorno.Any(x => x.Name == result.Trim()))
                    retorno.Add(new EquipamentViewModel { Name = result.Trim(), Id = item.Id });

            }

            return retorno;
        }
    }
}
