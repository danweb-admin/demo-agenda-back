using System;
namespace Solucao.Application.Contracts.Requests
{
	public class DashboardRequest
	{
		public string StartDate { get; set; }
		public string EndDate { get; set; }
		public string Status { get; set; }
	}
}

