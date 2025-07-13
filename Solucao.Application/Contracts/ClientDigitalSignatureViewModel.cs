using System;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Contracts
{
	public class ClientDigitalSignatureViewModel
	{
        public Guid? Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PartyName { get; set; }
        public bool IsPF { get; set; }
        public Guid ClientId { get; set; }
        public ClientViewModel Client { get; set; }
    }
}

