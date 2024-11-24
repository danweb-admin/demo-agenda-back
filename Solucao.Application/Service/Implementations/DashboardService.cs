using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Solucao.Application.Contracts;
using Solucao.Application.Data.Interfaces;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Service.Interfaces;

namespace Solucao.Application.Service.Implementations
{
    public class DashboardService : IDashboardService
    {
        private DashboardRepository repository;

        public DashboardService(DashboardRepository _repository)
        {
            repository = _repository;
        }

        public async Task<IEnumerable<SeriesDataDriverOrEquipmentViewModel>> EquipmentByPeriod(DateTime startDate, DateTime endDate, string status)
        {

            var result = await repository.DashboardGetCalendarByPeriodAndStatusAndDriver(startDate, endDate, status);

            var query = result
                    .GroupBy(l => l.Equipament.Name)
                    .Select(g => new SeriesDataDriverOrEquipmentViewModel
                    {
                        Name = g.Key,
                        Total = g.Count()
                    })
                    .OrderByDescending(x => x.Total)
                    .Take(5)
                    .ToList();

            return query;

        }

        public async Task<IEnumerable<SeriesDataDriverOrEquipmentViewModel>> DriverByPeriod(DateTime startDate, DateTime endDate, string status)
        {

            var result = await repository.DashboardGetCalendarByPeriodAndStatusAndDriver(startDate, endDate, status);

            var query = result
                    .GroupBy(l => l.Driver.Name)
                    .Select(g => new SeriesDataDriverOrEquipmentViewModel
                    {
                        Name = g.Key,
                        Total = g.Count()
                    })
                    .OrderByDescending(x => x.Total)
                    .Take(5)
                    .ToList();

            return query;

        }

        public async Task<IEnumerable<SeriesDataViewModel>> LocacoesByPeriod(DateTime startDate, DateTime endDate, string status)
        {
            var result = await repository.DashboardGetCalendarByPeriodAndStatus(startDate, endDate, status);

            var _status = status.Split(",");

            var list = new List<SeriesDataViewModel>();

            List<DateTime> dateList = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                    .Select(offset => startDate.AddDays(offset))
                                    .ToList();


            foreach (var item in _status)
            {
                var series = new SeriesDataViewModel();

                series.Name = returnStatus(item);

                List<int> ints = new List<int>();
                List<string> dates = new List<string>();

                foreach (var date in dateList)
                {
                    var dayValues = result.Count(x => x.Status == item && x.Date.Date == date);
                    ints.Add(dayValues);
                    dates.Add(date.ToString("dd/MM"));
                }


                series.Values = ints;
                series.Labels = dates;
                list.Add(series);

            }



            return list;
        }

        private string returnStatus(string status)
        {
            switch (status)
            {
                case "1":
                    return "Confirmada";
                case "2":
                    return "Pendente";
                case "3":
                    return "Cancelada";
                case "4":
                    return "Excluída";
                default:
                    return "Pré Agendada";

            }
        }
    }
}

