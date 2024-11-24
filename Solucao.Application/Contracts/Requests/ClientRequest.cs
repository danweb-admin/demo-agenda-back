using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Contracts.Requests
{
    public class ClientRequest
    {
        public bool Ativo { get; set; }
        public string Search { get; set; } = string.Empty;
        public Guid Id { get; set; }
    }
}
