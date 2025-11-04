using System;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Contracts
{
	public class PriceTableViewModel
	{
        public Guid? Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; }
        public Guid EquipmentId { get; set; }
        public decimal Value { get; set; }
        public int Minutes { get; set; }
        public EquipamentViewModel Equipment { get; set; }
    }
}

