using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Data.Entities
{
    public class Client : BaseEntity
    {
        public string Name { get; set; }
        public string CellPhone { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string  Number { get; set; }
        public string Neighborhood { get; set; }
        public string Complement { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public bool IsPhysicalPerson { get; set; }
        public bool? IsAnnualContract { get; set; }
        public int? IsReceipt { get; set; }
        public string NameForReceipt { get; set; }
        public bool? HasAirConditioning { get; set; }
        public bool? Has220V { get; set; }
        public bool? HasStairs { get; set; }
        public bool? TakeTransformer { get; set; }
        public bool? HasTechnique { get; set; }
        public string TechniqueOption1 { get; set; }
        public string TechniqueOption2 { get; set; }
        public string LandMark { get; set; }
        public string Responsible { get; set; }
        public string Specialty { get; set; }
        public string ClinicName { get; set; }
        public string ClinicCellPhone { get; set; }
        public string ZipCode { get; set; }
        public string Secretary { get; set; }
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
        public string Rg { get; set; }
        public string Ie { get; set; }
        public string EquipamentValues { get; set; }
        public decimal Discount { get; set; }
        public decimal Freight { get; set; }
        public City City { get; set; }
        public State State { get; set; }
        public ICollection<ClientEquipment> ClientEquipment { get; set; }
        public ICollection<ClientSpecification> ClientSpecifications { get; set; }


        public Client()
        {
            ClientEquipment = new List<ClientEquipment>();
            ClientSpecifications = new List<ClientSpecification>();
        }


    }
}
