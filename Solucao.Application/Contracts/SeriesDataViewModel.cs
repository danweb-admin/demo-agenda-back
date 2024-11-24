using System;
using System.Collections.Generic;

namespace Solucao.Application.Contracts
{
	public class SeriesDataViewModel
	{
        public string Name { get; set; }
        public List<int> Values { get; set; }
        public List<string> Labels { get; set; }
    }
}

