using System;
using Solucao.Application.Data.Entities;
using System.Collections.Generic;

namespace Solucao.Application.Contracts
{
	public class ClientEquipmentViewModel
	{
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid EquipmentRelationshipId { get; set; }
        public string Name { get; set; }
        public ClientViewModel Client { get; set; }
        public EquipmentRelationshipViewModel EquipmentRelationship { get; set; }
        public ICollection<TimeValueViewModel> TimeValues { get; set; }
    }
}

