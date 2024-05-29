using Microsoft.EntityFrameworkCore;
using Solucao.Application.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using NetDevPack.Data;
using Microsoft.Extensions.Logging;
using Solucao.Application.Utils.Enum;
using System.Linq;

namespace Solucao.Application.Data.Repositories
{
    public class UserRepository 
    {
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<User> DbSet;
        private readonly ILogger<UserRepository> logger;

        public UserRepository(SolucaoContext _context, ILogger<UserRepository> _logger)
        {
            Db = _context;
            DbSet = Db.Set<User>();
            logger = _logger;
        }

        public virtual async Task<IEnumerable<User>> GetAll(bool isDriver)
        {
            if (isDriver)
                return await Db.Users.Where(x => x.Role == "driver").ToListAsync();
            return await Db.Users.ToListAsync();
        }

        public virtual async Task<User> GetById(Guid Id)
        {
            return await Db.Users.FindAsync(Id);
        }


        public virtual async Task<User> GetByName(string name)
        {
            return await Db.Users.FirstOrDefaultAsync(x => x.Name == name);
        }

        public virtual async Task<User> Add(User user)
        {
            try
            {
                await Db.Users.AddAsync(user);
                Db.SaveChanges();

                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        
        public virtual async Task<User> Update(User user)
        {
            DbSet.Update(user);
            await Db.SaveChangesAsync();

            return user;
        }

        public virtual async Task<User> GetByEmail(string email)
        {
            return await Db.Users.FirstOrDefaultAsync(x => x.Email == email && x.Active);

        }

    }
}
