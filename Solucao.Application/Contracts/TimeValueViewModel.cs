using System;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Contracts
{
	public class TimeValueViewModel
	{
        public Guid Id { get; set; }
        public string Time { get; set; }
        public TimeSpan Time_ { get; set; }
        public decimal Value { get; set; }
        public Guid ClientEquipmentId { get; set; }
        public ClientEquipmentViewModel ClientEquipment { get; set; }
    }
}

