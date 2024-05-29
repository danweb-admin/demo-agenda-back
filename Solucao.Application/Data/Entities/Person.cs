using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Data.Entities
{
    public class Person : BaseEntity
    {
        public string Name { get; set; }
        public string Plate { get; set; }
        public string CellPhone { get; set; }
        public string PersonType { get; set; }
        public Guid? UserId { get; set; }
        public User User { get; set; }

    }
}
