using System;
using Solucao.Application.Contracts.Requests;

namespace Solucao.Application.Contracts.Requests
{
	public class CalendarReportRequest
	{
		public DateTime DataInicial { get; set; }
		public DateTime	DataFinal { get; set; }
		public Guid? EquipmentId { get; set; }
		public Guid? ClientId { get; set; }
		public string Status { get; set; }
	}
}
