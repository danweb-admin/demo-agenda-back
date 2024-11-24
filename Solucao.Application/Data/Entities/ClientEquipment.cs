using System;
using System.Collections.Generic;

namespace Solucao.Application.Data.Entities
{
	public class ClientEquipment
	{
		public Guid Id { get; set; }
		public Guid ClientId { get; set; }
		public Guid EquipmentRelationshipId { get; set; }
		public Client Client { get; set; }
		public EquipmentRelationship EquipmentRelationship { get; set; }
		public ICollection<TimeValue> TimeValues { get; set; }
	}
}

