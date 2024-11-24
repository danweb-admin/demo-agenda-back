using System;
namespace Solucao.Application.Contracts.Response
{
	public class ClientEquipmentNamesViewModel
	{
		public Guid ClientId { get; set; }
		public string ClientName { get; set; }
		public string EquipNames { get; set; }
		public string[] EquipmentId { get; set; }
	}
}

