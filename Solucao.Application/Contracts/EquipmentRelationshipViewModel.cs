using System;
using System.Collections.Generic;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Contracts
{
	public class EquipmentRelationshipViewModel
	{
        public Guid? Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public List<EquipmentRelationshipEquipment> equipmentRelationshipEquipment { get; set; }
    }
}

