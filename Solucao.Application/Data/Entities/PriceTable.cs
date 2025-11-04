using System;
namespace Solucao.Application.Data.Entities
{
	public class PriceTable : BaseEntity
	{
		public Guid EquipmentId { get; set; }
		public decimal Value { get; set; }
		public int Minutes { get; set; }
		public Equipament Equipment { get; set; }
	}
}

