using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Repositories
{
    public class ClientSpeficationRepository
	{
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<ClientSpecification> DbSet;

        public ClientSpeficationRepository(SolucaoContext _context)
		{
            Db = _context;
            DbSet = Db.Set<ClientSpecification>();
        }

        public virtual async Task<ValidationResult> RemoveAllByClient(Guid clientId)
        {
            try
            {
                var clientSpecifications = await Db.ClientSpecifications.Where(x => x.ClientId == clientId).ToListAsync();

                foreach (var item in clientSpecifications)
                {
                    Db.Entry(item).State = EntityState.Deleted;
                }

                

                return ValidationResult.Success;

            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }

        }
    }
}

