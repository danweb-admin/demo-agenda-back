using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Repositories
{
	public class ClientEquipmentRepository
	{
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<ClientEquipment> DbSet;

        public ClientEquipmentRepository(SolucaoContext _context)
        {
            Db = _context;
            DbSet = Db.Set<ClientEquipment>();
        }


        public virtual async Task<IEnumerable<ClientEquipment>> GetAllByClient(Guid clientId)
        {
            return await Db.ClientEquipment.Include(x => x.EquipmentRelationship).Where(x => x.ClientId == clientId).ToListAsync();
        }

        public virtual async Task<ValidationResult> RemoveAllByClient(Guid clientId)
        {
            try
            {
                var clientEquipment = await GetAllByClient(clientId);

                foreach (var item in clientEquipment)
                {
                    var temp = await Db.TimeValues.Where(x => x.ClientEquipmentId == item.Id).ToListAsync();
                    Db.TimeValues.RemoveRange(temp);

                }

                Db.ClientEquipment.RemoveRange(clientEquipment);

                await Db.SaveChangesAsync();
                
                return ValidationResult.Success;
                    
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
            
        }


        public virtual async Task Add(ClientEquipment clientEquipment)
        {
            try
            {

                Db.ClientEquipment.Add(clientEquipment);
                await Db.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

