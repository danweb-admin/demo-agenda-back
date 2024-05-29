using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Contracts
{
    public class PersonViewModel
    {
        public Guid? Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Plate { get; set; }
        public string CellPhone { get; set; }
        public string PersonType { get; set; }
        public Guid? UserId { get; set; }
    }
}
