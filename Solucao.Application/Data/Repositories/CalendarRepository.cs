using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Contracts.Response;
using Solucao.Application.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Solucao.Application.Data.Repositories
{
    public class CalendarRepository
    {
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<Calendar> DbSet;
        private List<string> notIn = new List<string> { "4" };

        public CalendarRepository(SolucaoContext _context)
        {
            Db = _context;
            DbSet = Db.Set<Calendar>();
        }

        public async Task<IEnumerable<Calendar>> GetAll(DateTime date)
        {

            return await Db.Calendars.Include(x => x.Equipament)
                                         .Include(x => x.Client.City)
                                         .Include(x => x.Client)
                                         .Include(x => x.Driver)
                                         .Include(x => x.DriverCollects)
                                         .Include(x => x.Technique)
                                         .Include(x => x.User)
                                         .Include(x => x.CalendarSpecifications)
                                         .Include(x => x.CalendarEquipamentConsumables)
                                         .ThenInclude(x => x.Consumable)
                                         .Include(x => x.CalendarSpecificationConsumables)
                                         .ThenInclude(x => x.Specification)
                                         .Where(x => x.Date.Date == date && x.Active && !notIn.Contains(x.Status))
                                         .OrderBy(x => x.Status).ToListAsync();

        }

        public async Task<IEnumerable<Calendar>> GetAllByDayAndConfirmed(DateTime date)
        {
            var confirmed = "1";

            return await Db.Calendars
                        .Include(x => x.Equipament)
                        .Include(x => x.Client)
                        .Where(x => x.Date.Date == date && x.Active && x.Status == confirmed)
                        .OrderBy(x => x.Equipament.Name)
                        .ToListAsync();

        }

        public async Task<Calendar> GetById(Guid id)
        {
            return await Db.Calendars
                        .Include(x => x.Equipament)
                        .Include(x => x.Client)
                        .ThenInclude(x => x.ClientDigitalSignatures)
                        .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Calendar> GetByUid(string uid)
        {
            return await Db.Calendars
                        .FirstOrDefaultAsync(x => x.Uid == uid);
        }


        public async Task<ValidationResult> Add(Calendar calendar)
        {
            try
            {

                Db.Calendars.Add(calendar);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }
        }

        public async Task<ValidationResult> Update(Calendar calendar)
        {
            try
            {
                DbSet.Update(calendar);
                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.Message);
            }

        }

        public async Task<IEnumerable<Calendar>> GetCalendarsByDate(DateTime date)
        {

            return await Db.Calendars
                .Include(x => x.CalendarSpecifications)
                .Where(x => x.Date.Date == date.Date && !notIn.Contains(x.Status) && x.Active)
                .OrderBy(x => x.Status)
                .OrderBy(x => x.Client.Name).ToListAsync();

        }

        public async Task<List<Calendar>> GetSpecificationsByDate(DateTime date, List<CalendarSpecifications> list)
        {
            var in_ = list.Select(x => x.SpecificationId);

            var result = await (from calendar in Db.Calendars
                    join specs in Db.CalendarSpecifications on calendar.Id equals specs.CalendarId
                    where in_.Contains(specs.SpecificationId) && calendar.Date.Date == date.Date &&
                    calendar.Active
                    select new Calendar
                    {
                        ClientId = calendar.ClientId,
                        EquipamentId = calendar.EquipamentId,
                        StartTime = calendar.StartTime,
                        EndTime = calendar.EndTime,
                        
                        
                    }).ToListAsync();

            return result;

        }

        public async Task<IEnumerable<Calendar>> ValidateEquipament(DateTime date, Guid clientId, Guid equipamentId)
        {
            
            var sql = $"select * from Calendars where date = '{date.ToString("yyyy-MM-dd")}' and equipamentId = '{equipamentId}' and ClientId != '{clientId}' and status not in ('3','4')";
            return await Db.Calendars.FromSqlRaw(sql).ToListAsync();

        }

        public async Task<IEnumerable<Calendar>> GetCalendarBySpecificationsAndDate(List<CalendarSpecifications> list, DateTime date, DateTime startTime)
        {
            var _in = In(list.Select(x => x.SpecificationId).ToList());

            var sql = $"select distinct c.* from Calendars as c left join CalendarSpecifications as cs on " +
                                "c.Id = cs.CalendarId " +
                                $"where CONVERT(varchar, c.date, 112) = '{date.ToString("yyyyMMdd")}' and ";
            if (list.Any())
                sql += $"cs.SpecificationId in ({_in}) and ";

            sql += $"'{startTime.ToString("HH:mm:ss")}' >= CONVERT(varchar, c.StartTime, 108) and " +
                                $"'{startTime.ToString("HH:mm:ss")}' <= CONVERT(varchar, c.EndTime, 108) ";

            return await Db.Calendars.FromSqlRaw(sql).ToListAsync();
        }

        public async Task<int> SpecCounterBySpec(Guid specificationId, DateTime date, DateTime startTime, Guid clientId)
        {
            var _notIn = new List<string> { "3", "4" };
            
            return await (from calendar in Db.Calendars
                           join specs in Db.CalendarSpecifications on calendar.Id equals specs.CalendarId
                           where specs.SpecificationId == specificationId
                           && calendar.Date.Date == date.Date
                           && startTime.TimeOfDay >= calendar.StartTime.Value.TimeOfDay
                           && startTime.TimeOfDay <= calendar.EndTime.Value.TimeOfDay
                           && specs.Active
                           && calendar.Active
                           && !_notIn.Contains(calendar.Status)
                           && calendar.ClientId != clientId
                           select specs).CountAsync();
        }


        public async Task<int> SingleSpecCounter(Guid specificationId, DateTime date)
        {
            var sql = $"select count(cs.Id) as amount from Calendars as c left join CalendarSpecifications as cs on " +
                                "c.Id = cs.CalendarId " +
                                $"where CONVERT(varchar, c.date, 112) = '{date.ToString("yyyyMMdd")}' and " +
                                $"cs.SpecificationId = '{specificationId}'";

            return await Db.Calendars.FromSqlRaw(sql).CountAsync();
        }


        public async Task<IEnumerable<Calendar>> Schedules(DateTime startDate, DateTime endDate,  Guid? clientId, List<Guid> equipamentId, List<Guid> driverId, Guid? techniqueId, string status)
        {
            try
            {
                var sql = await Db.Calendars.Include(x => x.Equipament)
                                  .Include(x => x.Client)
                                  .Include(x => x.Client.City)
                                  .Include(x => x.Client.State)
                                  .Include(x => x.Driver)
                                  .Include(x => x.Technique)
                                  .Include(x => x.User)
                                  .Include(x => x.CalendarSpecifications)
                                  .Where(x => x.Date.Date >= startDate
                                  && x.Date.Date <= endDate
                                  && x.Active).ToListAsync();

                if (clientId.HasValue)
                    sql = sql.Where(x => x.ClientId == clientId.Value).ToList();


                if (equipamentId.Any())
                    sql = sql.Where(x => equipamentId.Contains(x.EquipamentId)).ToList();

                if (driverId.Any())
                    sql = sql.Where(x => x.DriverId != null).Where(x => driverId.Contains(x.DriverId.Value)).ToList();

                if (techniqueId.HasValue)
                    sql = sql.Where(x => x.TechniqueId == techniqueId.Value).ToList();

                if (!string.IsNullOrEmpty(status))
                {
                    var _status = status.Split(",");
                    sql = sql.Where(x => _status.Contains(x.Status)).ToList();
                }
                    

                return sql.OrderBy(x => x.StartTime);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<IEnumerable<Calendar>> Availability(List<Guid> equipamentIds, int month, int year)
        {
            var _notIn = new List<string> { "3", "4" };

            try
            {
                var sql = await Db.Calendars
                                  .Include(x => x.CalendarSpecifications)
                                  .Where(x => x.Date.Month == month && x.Date.Year == year
                                  && !_notIn.Contains(x.Status)
                                  && x.Active && equipamentIds.Contains(x.EquipamentId)).ToListAsync();

                
                return sql;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<CalendarReportResponse>> CalendarReport(DateTime dataIncial, DateTime dataFinal, Guid? clientId, Guid? equipmentId, string status)
        {
            var sql = "select " +
                        "c.DATE as Date," +
                        "cli.Name as ClientName," +
                        "e.Name as EquipmentName," +
                        "c.Discount," +
                        "c.Freight," +
                        "c.Others," +
                        "c.[Value]," +
                        "c.TotalValue," +
                        "case when c.paymentStatus = 'pending' then 'Pendente'" +
                        "     when c.paymentStatus = 'paid' then 'Pago'" +
                        "     else  '' end as PaymentStatus," +
                        "c.paymentMethods as PaymentMethods," +
                        "case when c.STATUS = 1 then 'Confirmada'" +
                        "     when c.STATUS = 2 then 'Pendente'" +
                        "     when c.STATUS = 3 then 'Cancelada'" +
                        "     when c.STATUS = 4 then 'Excluída'" +
                        "     else  'Pré-Agendada'" +
                        "     end as Status " +
                        "from calendars as c " +
                        "inner join Clients as cli on c.ClientId = cli.Id " +
                        "inner join Equipaments as e on c.EquipamentId = e.Id " +
                        "where " +
                        $@"[Date] BETWEEN '{dataIncial.ToString("yyyy-MM-dd")}' and '{dataFinal.ToString("yyyy-MM-dd")}' AND " +
                        "c.Active = 1 and " +
                        "(c.ClientId = " + (clientId.HasValue ? $"'{clientId.Value}'" : "null" ) + " or " + (clientId.HasValue ? $"'{clientId.Value}'" : "null" ) +  " is null) and " +
                        $@"(c.EquipamentId = " + (equipmentId.HasValue ? $"'{equipmentId.Value}'" : "null") + " or " + (equipmentId.HasValue ? $"'{equipmentId.Value}'" : "null" ) +  " is null) AND " +
                        $@"(c.[Status] = {(string.IsNullOrEmpty(status) ? "''" : $"'{status}'")} or {(string.IsNullOrEmpty(status) ? "''" : $"'{status}'")} = '') " +
                        "order by " +
                        "c.[Date], cli.Name";

            return await Db.CalendarReports.FromSqlRaw(sql).ToListAsync();
        }

        public async Task<IEnumerable<CalendarViewResponse>> CalendarView(DateTime startDate, DateTime endDate)
        {
            var sql = "select " +
                        "starttime as start," +
                        "endtime as 'end'," +
                        "e.name + ' - ' + convert(varchar(15),cli.Name)  as title, " +
                        "e.name as EquipamentoFull, " +
                        "cli.Name + ' - ' + CONVERT(varchar(15),ci.Nome) + ' - ' + s.Sigla as ClienteFull, " +
                        "c.status, " +
                        "p1.Name as MotoristaRecolhe, " +
                        "p.Name as MotoristaEntrega, " +
                        "e.Color, " + 
                        "cli.CellPhone " +
                    "from Calendars as c " +
                    "inner join Equipaments as e on c.EquipamentId = e.id " +
                    "inner join Clients as cli on c.ClientId = cli.Id " +
                    "inner join Cities as ci on cli.CityId = ci.Id " +
                    "inner join States as s on ci.StateId = s.Id " +
                    "left join People as p on c.DriverId = p.Id " +
                    "left join People as p1 on c.DriverCollectsId = p1.Id " +
                    "where " +
                    "c.Active = 1 AND " +
                    "c.status in (1,2) AND " +
                    $@"[Date] BETWEEN '{startDate.ToString("yyyy-MM-dd")}' and '{endDate.ToString("yyyy-MM-dd")}'   " +
                    "order by [date]";

            return await Db.CalendarViewResponses.FromSqlRaw(sql).ToListAsync();
        }

        private string In(List<Guid> list)
        {
            var join = new List<string>();
            foreach (var item in list)
            {
                join.Add("'" + item + "'");
            }
            return string.Join(",",join);
        }

    }
}
