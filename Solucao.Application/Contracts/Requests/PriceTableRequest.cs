using System;
namespace Solucao.Application.Contracts.Requests
{
	public class PriceTableRequest
	{
		public Guid EquipmentId { get; set; }
        public string StartTime { get; set; }
		public string EndTime { get; set; }

    }
}

