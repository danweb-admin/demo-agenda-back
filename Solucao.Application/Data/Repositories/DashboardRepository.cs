using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Data.Repositories
{
    public class DashboardRepository
    {
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;

        public DashboardRepository(SolucaoContext _context)
        {
            Db = _context;
        }

        public async Task<IEnumerable<Calendar>> DashboardGetCalendarByPeriodAndStatus(DateTime startDate, DateTime endDate, string status)
        {
            var _in = status.Split(",");

            var results = await Db.Calendars
                .Where(c => c.Date >= startDate && c.Date <= endDate
                            && (_in.Contains(c.Status)) && c.Active).ToListAsync();

            return results;
        }

        public async Task<IEnumerable<Calendar>> DashboardGetCalendarByPeriodAndStatusAndDriver(DateTime startDate, DateTime endDate, string status)
        {
            var _in = status.Split(",");

            var results = await Db.Calendars
                .Include(x => x.Driver)
                .Include(x => x.Equipament)
                .Where(c => c.Date >= startDate && c.Date <= endDate
                            && (_in.Contains(c.Status) && c.Driver != null) && c.Active).ToListAsync();

            return results;
        }
    }

}

