using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solucao.Application.Data.Entities;

namespace Solucao.Application.Contracts
{
    public class ClientViewModel
    {
        public Guid? Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string CellPhone { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        public string Neighborhood { get; set; }
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
        public string Document { get; set; }

        public object City { get; set; }
        public object State { get; set; }
        public ICollection<ClientEquipmentViewModel> ClientEquipment { get; set; }
        public ICollection<ClientSpecificationViewModel> ClientSpecifications { get; set; }
        public ICollection<ClientDigitalSignatureViewModel> ClientDigitalSignatures { get; set; }


        public ClientViewModel()
        {
            ClientEquipment = new List<ClientEquipmentViewModel>();
            ClientSpecifications = new List<ClientSpecificationViewModel>();
            ClientDigitalSignatures = new List<ClientDigitalSignatureViewModel>();
        }


    }
}
