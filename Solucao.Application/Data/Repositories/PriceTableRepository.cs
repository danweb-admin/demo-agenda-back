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
	public class PriceTableRepository
	{
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<PriceTable> DbSet;

        public PriceTableRepository(SolucaoContext _context)
        {
            Db = _context;
            DbSet = Db.Set<PriceTable>();
        }

        public async Task<IEnumerable<PriceTable>> GetAll(Guid equipmentId)
        {
            
                return await Db.PriceTable.Where(x => x.Active  && x.EquipmentId == equipmentId).ToListAsync();
        }

        public async Task<ValidationResult> Add(PriceTable priceTable)
        {
            try
            {

                Db.PriceTable.Add(priceTable);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
        }

        public async Task<ValidationResult> AddRange(List<PriceTable> priceTable)
        {
            try
            {

                await Db.PriceTable.AddRangeAsync(priceTable);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
        }

        public async Task<ValidationResult> Update(PriceTable priceTable)
        {
            try
            {
                DbSet.Update(priceTable);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }

        }

        public async Task<ValidationResult> SaveChange()
        {
            try
            {
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }

        }

    }
}

