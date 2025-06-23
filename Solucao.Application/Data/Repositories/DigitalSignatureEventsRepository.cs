using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Repositories
{
	public class DigitalSignatureEventsRepository
	{
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<DigitalSignatureEvents> DbSet;


        public DigitalSignatureEventsRepository(SolucaoContext _context)
        {
            Db = _context;
            DbSet = Db.Set<DigitalSignatureEvents>();
        }

        public async Task<ValidationResult> Add(DigitalSignatureEvents evento)
        {
            try
            {

                Db.DigitalSignatureEvents.Add(evento);
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

