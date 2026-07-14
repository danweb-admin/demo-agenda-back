using System;
using Solucao.Application.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Solucao.Application.Contracts.Requests;

namespace Solucao.Application.Service.Interfaces
{
  public interface IGoogleService
  {
    Task ExtrairInformacoe(GoogleRequest request);

  }
}

