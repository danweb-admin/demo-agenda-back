using System;
namespace Solucao.Application.Data.Entities
{
	public class TimeValue
	{
		public Guid Id { get; set; }
		public string Time { get; set; }
		public decimal Value { get; set; }
		public Guid ClientEquipmentId { get; set; }
		public ClientEquipment ClientEquipment { get; set; }
	}
}

