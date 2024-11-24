using System;
namespace Solucao.Application.Data.Entities
{
	public class EquipmentRelationshipEquipment
	{
		public Guid Id { get; set; }
		public Guid EquipmentRelationshipId { get; set; }
        public Guid EquipmentId { get; set; }
		public EquipmentRelationship EquipmentRelationship { get; set; }
		public Equipament Equipament { get; set; }
	}
}

