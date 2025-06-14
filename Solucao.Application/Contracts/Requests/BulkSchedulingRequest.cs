using System;
using Solucao.Application.Data.Entities;
using System.Collections.Generic;

namespace Solucao.Application.Contracts.Requests
{
    public class BulkSchedulingRequest
    {
        public Guid ClientId { get; set; }
        public Guid EquipmentId { get; set; }
        public string Date { get; set; }
        public string StartTime1 { get; set; }
        public string EndTime1 { get; set; }
        public string Note { get; set; }
        public bool CheckScheduling { get; set; }
        public IList<CalendarSpecifications> CalendarSpecifications { get; set; }
    }
}

