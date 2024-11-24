using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Repositories
{
	public class TimeValuesRepository
	{
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<TimeValue> DbSet;

        public TimeValuesRepository(SolucaoContext _context)
        {
            Db = _context;
            DbSet = Db.Set<TimeValue>();
        }
        
        public virtual async Task Add(List<TimeValue> timeValue)
        {
            try
            {

                Db.TimeValues.AddRange(timeValue);
                await Db.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public virtual async Task<TimeValue> GetTimeValue(string time, string equipmentName, Guid clientId)
        {
            try
            {
                var sql = $"select tv.* from TimeValues as tv inner join ClientEquipment as ce on tv.ClientEquipmentId = ce.Id inner join EquipmentRelantionships as er on er.id = ce.EquipmentRelationshipId where tv.[Time] = '{time}' and ce.ClientId = '{clientId}' and er.Name = '{equipmentName}'";
                return await Db.TimeValues.FromSqlRaw(sql).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public virtual async Task Update(TimeValue timeValue)
        {
            try
            {

                Db.TimeValues.Update(timeValue);
                await Db.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

