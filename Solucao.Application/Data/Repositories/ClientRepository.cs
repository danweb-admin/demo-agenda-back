using DocumentFormat.OpenXml.InkML;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Contracts;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Data.Repositories
{
    public class ClientRepository : IClientRepository
    {
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<Client> DbSet;

        public ClientRepository(SolucaoContext _context)
        {
            Db = _context;
            DbSet = Db.Set<Client>();
        }

        public async Task<IEnumerable<Client>> GetAll(bool ativo, string search)
        {
            if (string.IsNullOrEmpty(search))
                return await Db.Clients
                    .Include(x => x.City)
                    .Include(x => x.State).Where(x => x.Active == ativo).OrderBy(x => x.Name).ToListAsync();

            return await Db.Clients
                .Include(x => x.City)
                .Include(x => x.State).Where(x => x.Active == ativo && (x.Address.Contains(search) || x.Email.Contains(search) || x.Name.Contains(search) || x.Phone.Contains(search) || x.CellPhone.Contains(search))).OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<Client> GetById(Guid Id)
        {
            return await Db.Clients
                    .Include(x => x.ClientEquipment)
                    .ThenInclude(x => x.EquipmentRelationship)
                    .Include(x => x.ClientEquipment)
                    .ThenInclude(x => x.TimeValues)
                    .Include(x => x.ClientSpecifications)
                    .ThenInclude(x => x.Specification)
                    .Include(x => x.ClientDigitalSignatures).FirstAsync(x => x.Id == Id);
            
        }

        public async Task<decimal> GetEquipmentValueByClient(Guid clientId, Guid equipmentId, string time)
        {
            var result = from c in Db.Clients
                         join ce in Db.ClientEquipment on c.Id equals ce.ClientId
                         join ere in Db.EquipmentRelationshipEquipment on ce.EquipmentRelationshipId equals ere.EquipmentRelationshipId
                         join tv in Db.TimeValues on ce.Id equals tv.ClientEquipmentId
                         where c.Id == clientId &&
                               ere.EquipmentId == equipmentId &&
                               tv.Time == time
                         select tv.Value;

            return await result.SingleOrDefaultAsync();
        }



        public async Task<ValidationResult> Add(Client client)
        {
            try
            {

                Db.Clients.Add(client);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
        }

        public async Task<ValidationResult> Update(ClientViewModel dto)
        {
            
            var client = await Db.Clients
                .Include(c => c.ClientEquipment)
                .ThenInclude(x => x.TimeValues)
                .Include(c => c.ClientSpecifications)
                .Include(c => c.ClientDigitalSignatures)
                .FirstOrDefaultAsync(c => c.Id == dto.Id);

            if (client == null)
                return new ValidationResult("Cliente não encontrado.");

            // Atualizar propriedades simples
            client.UpdatedAt = DateTime.UtcNow;
            client.Active = dto.Active;
            client.Name = dto.Name;
            client.CellPhone = dto.CellPhone;
            client.Phone = dto.Phone;
            client.Email = dto.Email;
            client.Address = dto.Address;
            client.Number = dto.Number;
            client.Complement = dto.Complement;
            client.Neighborhood = dto.Neighborhood;
            client.CityId = dto.CityId;
            client.StateId = dto.StateId;
            client.IsPhysicalPerson = dto.IsPhysicalPerson;
            client.IsAnnualContract = dto.IsAnnualContract;
            client.IsReceipt = dto.IsReceipt;
            client.NameForReceipt = dto.NameForReceipt;
            client.HasAirConditioning = dto.HasAirConditioning;
            client.Has220V = dto.Has220V;
            client.HasStairs = dto.HasStairs;
            client.TakeTransformer = dto.TakeTransformer;
            client.HasTechnique = dto.HasTechnique;
            client.TechniqueOption1 = dto.TechniqueOption1;
            client.TechniqueOption2 = dto.TechniqueOption2;
            client.LandMark = dto.LandMark;
            client.Responsible = dto.Responsible;
            client.Specialty = dto.Specialty;
            client.ClinicName = dto.ClinicName;
            client.ClinicCellPhone = dto.ClinicCellPhone;
            client.ZipCode = dto.ZipCode;
            client.Secretary = dto.Secretary;
            client.Cpf = dto.Cpf;
            client.Cnpj = dto.Cnpj;
            client.Rg = dto.Rg;
            client.Ie = dto.Ie;
            client.EquipamentValues = dto.EquipamentValues;

            // Remove todos os equipamentos e TimeValues anteriores
            foreach (var equipamento in client.ClientEquipment.ToList())
            {
                Db.TimeValues.RemoveRange(equipamento.TimeValues); // Remove os TimeValues
            }


            // Adiciona os novos equipamentos e TimeValues
            foreach (var equipamento in dto.ClientEquipment.ToList())
            {
                foreach (var item in equipamento.TimeValues)
                {
                    var timeValue = new TimeValue
                    {
                        Id = Guid.NewGuid(),
                        Time = item.Time,
                        Value = item.Value,
                        ClientEquipmentId = equipamento.Id
                    };
                    Db.TimeValues.Add(timeValue);
                }
            }

            Db.ClientSpecifications.RemoveRange(client.ClientSpecifications);
            foreach (var item in dto.ClientSpecifications)
            {
                var ponteira = new ClientSpecification
                {
                    Id = Guid.NewGuid(),
                    ClientId = client.Id,
                    SpecificationId = item.SpecificationId,
                    Hours = item.Hours,
                    Value = item.Value
                };
                Db.ClientSpecifications.Add(ponteira);

            }
                

            // Remove todos os signatarios
            Db.ClientDigitalSignatures.RemoveRange(client.ClientDigitalSignatures);
            foreach (var item in dto.ClientDigitalSignatures)
            {
                var signatario = new ClientDigitalSignature
                {
                    Id = Guid.NewGuid(),
                    ClientId = client.Id,
                    Active = item.Active,
                    Email = item.Email,
                    IsPF = item.IsPF,
                    Name = item.Name,
                    PartyName = item.PartyName,
                    CreatedAt = DateTime.Now

                };
                Db.ClientDigitalSignatures.Add(signatario); 
            }

            Db.Entry(client).State = EntityState.Modified;
            await Db.SaveChangesAsync();
            return ValidationResult.Success;

        }



        public async Task<ValidationResult> AddClientEquipmentAndTimeValues(Client client)
        {
            try
            {
                Db.ClientEquipment.AddRange(client.ClientEquipment);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
        }

        public async Task<IEnumerable<ClientSpecification>> GetSpecsByClient(Guid clientId)
        {
            return await Db.ClientSpecifications.Where(x => x.ClientId == clientId).OrderBy(x => x.Hours).ToListAsync();
        }
    }
}
