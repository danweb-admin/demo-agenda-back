using System;
namespace Solucao.Application.Exceptions.Integration
{
  public class IntegrationException : Exception
  {
    
      public IntegrationException()
      {
      }

      public IntegrationException(string message)
          : base(message)
      {
      }
  }
}

