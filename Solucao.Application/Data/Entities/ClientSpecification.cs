using System;
namespace Solucao.Application.Data.Entities
{
	public class ClientSpecification 
	{
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid SpecificationId { get; set; }
        public int Hours { get; set; }
        public decimal Value { get; set; }
        public Client Client { get; set; }
        public Specification Specification { get; set; }
    }
}

