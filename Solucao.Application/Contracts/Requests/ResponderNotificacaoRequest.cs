using System;
namespace Solucao.Application.Contracts.Requests
{
  public class ResponderNotificacaoRequest
  {
    public string Token { get; set; }
    public string Resposta { get; set; } // Confirmado / Cancelado
  }
}

