using System;
namespace Solucao.Application.Contracts.Response
{
	public class CalendarViewResponse
	{
		public Guid? CalendarId { get; set; }
		public string Title { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
		public string Status { get; set; }
		public string EquipamentoFull { get; set; }
		public string ClienteFull { get; set; }
		public string MotoristaRecolhe { get; set; }
        public string MotoristaEntrega { get; set; }
		public string Color { get; set; }
		public string CellPhone { get; set; }
		public string Endereco { get; set; }
		public string Note { get; set; }


	}
}

