using System;
namespace Solucao.Application.Exceptions.DigitalSignature
{
	public class DigitalSignatureException : Exception
    {
		public DigitalSignatureException()
		{
		}

        public DigitalSignatureException(string message)
            : base(message)
        {

        }
    }
}

