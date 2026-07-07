using Dynamitey;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Solucao.Application.Data.Repositories
{
    public class LogisticsRepository
    {
        public IUnitOfWork UnitOfWork => Db;

        protected readonly SolucaoContext Db;
        protected readonly DbSet<Logistics> DbSet;

        public LogisticsRepository(SolucaoContext context)
        {
            Db = context;
            DbSet = Db.Set<Logistics>();
        }

        public async Task<IEnumerable<Logistics>> GetAll()
        {
            return await DbSet
                .Include(x => x.Calendar)
                    .ThenInclude(x => x.Client)
                .Include(x => x.Calendar)
                    .ThenInclude(x => x.Equipament)
                .Include(x => x.Driver)
                .Where(x => x.Active)
                .OrderBy(x => x.DataHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<Logistics>> GetByDate(DateTime data)
        {
            return await DbSet
                  .Include(x => x.Driver)
                  .Include(x => x.Calendar)
                      .ThenInclude(x => x.Client)
                  .Include(x => x.Calendar)
                      .ThenInclude(x => x.Equipament)
                  .Include(x => x.Calendar)
                      .ThenInclude(x => x.Client.City)
                  .Where(x => x.Active && x.DataHora.Date == data.Date)
                  .OrderBy(x => x.DataHora)
                  .ToListAsync();
        }

        public async Task<IEnumerable<Logistics>> GetByDriver(Guid driverId, DateTime data)
        {
            return await DbSet
                .Include(x => x.Calendar)
                    .ThenInclude(x => x.Client)
                .Include(x => x.Calendar)
                    .ThenInclude(x => x.Equipament)
                .Include(x => x.Driver)
                .Where(x => x.Active &&
                            x.DriverId == driverId &&
                            x.DataHora.Date == data.Date)
                .OrderBy(x => x.DataHora)
                .ToListAsync();
        }

        public async Task<Logistics> GetById(Guid id)
        {
            return await DbSet
                .Include(x => x.Calendar)
                    .ThenInclude(x => x.Client)
                .Include(x => x.Calendar)
                    .ThenInclude(x => x.Equipament)
                .Include(x => x.Driver)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ValidationResult> Add(Logistics logistics)
        {
            try
            {
                Db.Entry(logistics).State = EntityState.Added;

                await Db.SaveChangesAsync();

                return ValidationResult.Success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ValidationResult> Update(Logistics logistics)
        {
            try
            {
                Db.Entry(logistics).State = EntityState.Modified;

                await Db.SaveChangesAsync();

                return ValidationResult.Success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ValidationResult> Remove(Guid id)
        {
            try
            {
                var logistics = await DbSet.FindAsync(id);

                if (logistics == null)
                    return ValidationResult.Success;

                logistics.Active = false;
                logistics.UpdatedAt = DateTime.Now;

                Db.Entry(logistics).State = EntityState.Modified;

                await Db.SaveChangesAsync();

                return ValidationResult.Success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Logistics> Complete(Guid id)
        {
            var entity = await DbSet.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return null;

            entity.Concluido = true;
            entity.UpdatedAt = DateTime.Now;

            Db.Entry(entity).State = EntityState.Modified;

            await Db.SaveChangesAsync();

            return entity;
        }

        public async Task<Logistics> Uncomplete(Guid id)
        {
            var entity = await DbSet.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return null;

            entity.Concluido = false;
            entity.UpdatedAt = DateTime.Now;

            Db.Entry(entity).State = EntityState.Modified;

            await Db.SaveChangesAsync();

            return entity;
        }

        public async Task RemoveByCalendar(Guid calendarId)
        {
            var eventos = await DbSet
                .Where(x => x.CalendarId == calendarId && x.Active)
                .ToListAsync();

            foreach (var evento in eventos)
            {
                evento.Active = false;
                evento.UpdatedAt = DateTime.Now;
                Db.Entry(evento).State = EntityState.Modified;

                
            }

            await Db.SaveChangesAsync();
        }
    }
}