using System;
using System.Collections.Generic;

namespace Solucao.Application.Data.Results
{
  public class ImportacaoClientesResult
  {
      public int TotalLinhas { get; set; }

      public int Importados { get; set; }

      public int Ignorados { get; set; }

      public List<string> Erros { get; set; } = new List<string>();
  }
}

