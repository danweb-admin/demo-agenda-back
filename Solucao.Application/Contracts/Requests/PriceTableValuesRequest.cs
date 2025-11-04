using System;
using Solucao.Application.Data.Entities;
using System.Collections.Generic;

namespace Solucao.Application.Contracts.Requests
{
	public class PriceTableValuesRequest
	{
		public Guid Id { get; set; }
        public IList<PriceTableViewModel> Valores { get; set; }
	}
}

