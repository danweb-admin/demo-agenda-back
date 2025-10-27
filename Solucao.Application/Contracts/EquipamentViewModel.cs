using Solucao.Application.Data.Entities;
using System;
using System.Collections.Generic;

namespace Solucao.Application.Contracts
{
    public class EquipamentViewModel
    {
        public Guid? Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string Color { get; set; }
        public List<EquipamentSpecifications> EquipamentSpecifications { get; set; }
        public List<EquipamentConsumableViewModel> EquipamentConsumables { get; set; }

    }
}
