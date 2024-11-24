using Solucao.Application.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Data.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAll(bool ativo, string search);
        Task<Client> GetById(Guid Id);
        Task<ValidationResult> Add(Client client);
        Task<ValidationResult> Update(Client client);
        Task<ValidationResult> AddClientEquipmentAndTimeValues(Client client);
        Task<decimal> GetEquipmentValueByClient(Guid clientId, Guid equipmentId, string time);
        Task<IEnumerable<ClientSpecification>> GetSpecsByClient(Guid clientId);

    }
}
