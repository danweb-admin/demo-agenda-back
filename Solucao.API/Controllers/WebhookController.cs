using System;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using NetDevPack.Messaging;

namespace Solucao.API.Controllers
{
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private const string Secret = "KnhT1u/dmZEhU2CA0+Sm9bGLwo/k45vFxoxNWfydBES+hbc7buhE4du5iJoOAFYJo5oMgVyWBJ7EgaKFZg8EiQ==";

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            // Lê o corpo bruto da requisição
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            Console.WriteLine(body);

            // Pega o HMAC enviado no header
            var signatureHeader = Request.Headers;

            var receivedHmac = Request.Headers["HMAC"].ToString();

            //if (string.IsNullOrEmpty(signatureHeader))
            //{
            //    return Unauthorized("Assinatura não encontrada");
            //}

            // Calcula o HMAC local
            var computedSignature = GenerateHmac(body, Secret);

            // 4. Comparar os valores
            if (string.Equals(computedSignature, receivedHmac, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Webhook autenticado com sucesso.");
                Console.WriteLine("Payload recebido: " + body);
                Console.WriteLine("X-Signature: " + signatureHeader);

                return Ok(new { message = "Webhook recebido e validado com sucesso!" });
            }
            else
            {
                return Unauthorized(new { error = "HMAC inválido" });
            }

            


            return Ok(new { status = "Recebido e autenticado" });
        }

        private string GenerateHmac(string payload, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var messageBytes = Encoding.UTF8.GetBytes(payload);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // Hexadecimal (sem hífen)
            }
        }
    }
}

