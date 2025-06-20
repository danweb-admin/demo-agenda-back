using System;
namespace Solucao.Application.Contracts
{
	public class DigitalSignatureViewModel
	{
        public Guid? Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; }
        public Guid IdProcesso { get; set; }
        public Guid IdPasta { get; set; }
        public Guid IdResponsavel { get; set; }
        public Guid CalendarId { get; set; }
        public string NomeProcesso { get; set; }
        public string Status { get; set; }
        public CalendarViewModel Calendar { get; set; }
    }
}

