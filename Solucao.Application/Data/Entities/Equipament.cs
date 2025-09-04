using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Data.Entities
{
    public class Equipament : BaseEntity
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string Color { get; set; }
        public List<EquipamentSpecifications> EquipamentSpecifications { get; set; }
        public List<EquipamentConsumable> EquipamentConsumables { get; set; }
    }
}
