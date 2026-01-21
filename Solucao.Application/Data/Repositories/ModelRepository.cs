using System;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Solucao.Application.Data.Repositories
{
	public class ModelRepository
	{
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<Model> DbSet;

        public ModelRepository(SolucaoContext _context)
        {
            Db = _context;
            DbSet = Db.Set<Model>();
        }

        public async Task<IEnumerable<Model>> GetAll()
        {
            return await Db.Models.Include(x => x.ModelAttributes).ToListAsync();
        }

        public async Task<Model> GetByEquipament(Guid equipamentId)
        {
            return await Db.Models
                        .Include(x => x.Equipament)
                        .Include(x => x.ModelAttributes)
                        .FirstOrDefaultAsync(x => x.EquipamentId == equipamentId && x.Active);
        }

        public async Task<ValidationResult> Add(Model model)
        {
            try
            {
                Db.Entry(model).State = EntityState.Added;

                foreach (var item in model.ModelAttributes)
                {
                    Db.Entry(item).State = EntityState.Added;
                }
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<ValidationResult> Update(Model model)
        {
            try
            {

                Db.Entry(model).State = EntityState.Modified;
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}

