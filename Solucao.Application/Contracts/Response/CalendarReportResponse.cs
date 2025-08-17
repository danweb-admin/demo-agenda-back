using System;
namespace Solucao.Application.Contracts.Response
{
	public class CalendarReportResponse
	{
		public DateTime Date { get; set; }
		public string ClientName { get; set; }
		public string EquipmentName { get; set; }
		public decimal Discount { get; set; }
		public decimal Freight { get; set; }
		public decimal Others { get; set; }
		public decimal Value { get; set; }
		public decimal TotalValue { get; set; }
		public string PaymentStatus { get; set; }
		public string PaymentMethods { get; set; }
		public string Status { get; set; }
	}
}

