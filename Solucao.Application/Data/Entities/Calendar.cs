using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Data.Entities
{
    public class Calendar
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid EquipamentId { get; set; }
        public string Note { get; set; }
        public Guid ClientId { get; set; }
        public string Status { get; set; }
        public bool Active { get; set; }
        public bool NoCadastre { get; set; }
        public string TemporaryName { get; set; }
        public DateTime Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Guid? DriverId { get; set; }
        public Guid? DriverCollectsId { get; set; }
        public Guid? TechniqueId { get; set; }
        public Guid UserId { get; set; }
        public int TravelOn { get; set; }
        public bool ContractMade { get; set; }
        public decimal Value { get; set; }
        public decimal TotalValue { get; set; }
        public decimal Discount { get; set; }
        public decimal Freight { get; set; }
        public int RentalTime { get; set; }
        public string ContractPath { get; set; }
        public string Uid { get; set; }
        public User User { get; set; }
        public Person Technique { get; set; }
        public Person Driver { get; set; }
        public Person DriverCollects { get; set; }
        public Client Client { get; set; }
        public Equipament Equipament { get; set; }
        public IList<CalendarSpecifications> CalendarSpecifications { get; set; }
        public IList<CalendarEquipamentConsumable> CalendarEquipamentConsumables { get; set; }
        public IList<CalendarSpecificationConsumables> CalendarSpecificationConsumables { get; set; }


    }
}
