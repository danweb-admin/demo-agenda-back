using System;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using System.Collections.Generic;
using Solucao.Application.Data.Entities;
using DocumentFormat.OpenXml.InkML;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Solucao.Application.Data.Repositories
{
	public class DigitalSignatureRepository
	{
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<DigitalSignature> DbSet;

        
        public DigitalSignatureRepository(SolucaoContext _context)
		{
            Db = _context;
            DbSet = Db.Set<DigitalSignature>();
        }

        public async Task<ValidationResult> Add(DigitalSignature assinatura)
        {
            try
            {

                Db.DigitalSignatures.Add(assinatura);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
        }

        public async Task<ValidationResult> Update(DigitalSignature assinatura)
        {
            try
            {

                Db.Entry(assinatura).State = EntityState.Modified;
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
        }

        public async Task<DigitalSignature> GetByCalendarId(Guid calendarId)
        {
            return await Db.DigitalSignatures
                        .FirstOrDefaultAsync(x => x.CalendarId == calendarId);
        }

        public async Task<DigitalSignature> GetById(Guid idProcesso)
        {
            return await Db.DigitalSignatures
                        .FirstOrDefaultAsync(x => x.IdProcesso == idProcesso);
        }
    }
}

