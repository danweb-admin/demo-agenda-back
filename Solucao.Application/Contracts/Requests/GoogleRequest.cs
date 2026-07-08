using System;
namespace Solucao.Application.Contracts.Requests
{
  public class GoogleRequest
  {
    public string Id { get; set; }
    public string Titulo { get; set; }
    public string Inicio { get; set; }
    public string Fim { get; set; }
    public string Local { get; set; }
    public string Descricao { get; set; }
    
  }
}

