using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Repositories
{
	public class ClientDigialSignatureRepository
	{
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<ClientDigitalSignature> DbSet;

        public ClientDigialSignatureRepository(SolucaoContext _context)
        {
            Db = _context;
            DbSet = Db.Set<ClientDigitalSignature>();
        }

        public async Task<ValidationResult> Add(ClientDigitalSignature client)
        {
            try
            {

                Db.ClientDigitalSignatures.Add(client);
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

