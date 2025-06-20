using System;
namespace Solucao.Application.Data.Entities
{
	public class DigitalSignature : BaseEntity
	{
		public Guid? IdProcesso { get; set; }
		public Guid IdPasta { get; set; }
		public Guid IdResponsavel { get; set; }
		public Guid CalendarId { get; set; }
		public string NomeProcesso { get; set; }
		public string Status { get; set; }
		public Calendar Calendar { get; set; }
	}
}

