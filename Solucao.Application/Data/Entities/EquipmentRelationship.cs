using System;
using System.Collections.Generic;

namespace Solucao.Application.Data.Entities
{
	public class EquipmentRelationship : BaseEntity
	{
		public string Name { get; set; }
		public List<EquipmentRelationshipEquipment> equipmentRelationshipEquipment { get; set; }
	}
}

