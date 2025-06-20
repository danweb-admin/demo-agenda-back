using System;
namespace Solucao.Application.Data.Entities
{
	public class ClientDigitalSignature : BaseEntity
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string PartyName { get; set; }
		public bool IsPF { get; set; }
		public Guid ClientId { get; set; }
		public Client Client { get; set; }
	}
}

