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
	public class EquipmentRelationshipRepository
	{
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<EquipmentRelationship> DbSet;

        

        public EquipmentRelationshipRepository(SolucaoContext _context)
        {
            Db = _context;
            DbSet = Db.Set<EquipmentRelationship>();
        }

        public virtual async Task<IEnumerable<EquipmentRelationship>> GetAll(bool ativo)
        {
            return await Db.EquipmentRelantionships
                .Include(x => x.equipmentRelationshipEquipment)
                .Where(x => x.Active == ativo).ToListAsync();
        }

        public virtual async Task<EquipmentRelationship> GetByName(string name)
        {
            return await Db.EquipmentRelantionships.FirstAsync(x => x.Name.Contains(name));
        }



        public virtual async Task<ValidationResult> Add(EquipmentRelationship equipament)
        {
            try
            {

                Db.EquipmentRelantionships.Add(equipament);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public virtual async Task<ValidationResult> Update(EquipmentRelationship equipament)
        {
            try
            {
                Db.EquipmentRelationshipEquipment.RemoveRange(Db.EquipmentRelationshipEquipment.Where(x => x.EquipmentRelationshipId == equipament.Id));
                DbSet.Update(equipament);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw;
            }

        }
    }
}

