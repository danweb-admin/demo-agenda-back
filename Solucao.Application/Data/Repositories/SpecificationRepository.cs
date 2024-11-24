using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Data.Repositories
{
    public class SpecificationRepository
    {
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<Specification> DbSet;

        public SpecificationRepository(SolucaoContext _context)
        {
            Db = _context;
            DbSet = Db.Set<Specification>();
        }

        public virtual async Task<IEnumerable<Specification>> GetAll()
        {
            return await Db.Specifications.Include(x => x.EquipamentSpecifications).OrderBy(x => x.Name).ToListAsync();
        }

        public virtual async Task<Specification> GetById(Guid Id)
        {
            return await Db.Specifications.FindAsync(Id);
        }

        public virtual async Task<Specification> GetSingleSpec()
        {
            return await Db.Specifications.FirstOrDefaultAsync(x => x.Single && x.Active);
        }

        public virtual async Task<bool> SingleIsValid(Guid? id)
        {
            if (id.HasValue)
                return await Db.Specifications.AnyAsync(x => x.Id != id.Value && x.Active && x.Single);
            return await Db.Specifications.AnyAsync(x => x.Active && x.Single);
        }

        public virtual async Task<List<Specification>> GetSpecificationByEquipament(List<Guid> equipamentIds)
        {

            return await (from specification in Db.EquipamentSpecifications
                          where
                          specification.Active && equipamentIds.Contains(specification.EquipamentId)
                          select specification.Specification).Distinct().OrderBy(x => x.Name).ToListAsync();
        }

        public virtual async Task<List<Equipament>> GetSpecificationByEquipament2(List<Guid> equipamentIds, List<Guid> specificationIds)
        {
            return await Db.EquipamentSpecifications.Where(x => 
                          x.Active && equipamentIds.Contains(x.EquipamentId) &&
                          specificationIds.Contains(x.SpecificationId)).Select(x => x.Equipament).ToListAsync();

        }

        public virtual async Task<ValidationResult> Add(Specification specification)
        {
            try
            {

                Db.Specifications.Add(specification);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
        }

        public virtual async Task<ValidationResult> Update(Specification specification)
        {
            try
            {
                DbSet.Update(specification);
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
