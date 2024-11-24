using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DevPack.Data;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Repositories
{
	public class ModelAttributesRepository
	{
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<ModelAttributes> DbSet;

        public ModelAttributesRepository(SolucaoContext _context)
		{
            Db = _context;
            DbSet = Db.Set<ModelAttributes>();
        }

        public async Task<IEnumerable<ModelAttributes>> GetAll()
        {
            return await Db.ModelAttributes.ToListAsync();
        }

        public async Task<ValidationResult> Add(ModelAttributes model)
        {
            try
            {

                Db.ModelAttributes.Add(model);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
        }

        public async Task<ValidationResult> Update(ModelAttributes model)
        {
            try
            {
                DbSet.Update(model);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }

        }

        

        public async Task<ValidationResult> Remove(Guid id)
        {
            try
            {
                var model = Db.ModelAttributes.Find(id);
                if (model == null)
                    return new ValidationResult("Não foi possível remover o objeto.");

                DbSet.Remove(model);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }

        }

        public async Task<ValidationResult> RemoveAll(IEnumerable<ModelAttributes> list)
        {
            try
            {
                DbSet.RemoveRange(list);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);

            }
        }
    }
}

