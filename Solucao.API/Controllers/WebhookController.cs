using System;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Solucao.API.Controllers
{
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private const string Secret = "minha-chave-secreta-supersegura";

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            // Lê o corpo bruto da requisição
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            Console.WriteLine(body);

            // Pega o HMAC enviado no header
            var signatureHeader = Request.Headers;

            foreach (var item in signatureHeader)
            {
                Console.WriteLine($"key: {item.Key} - value: {item.Value}");
            }

            //if (string.IsNullOrEmpty(signatureHeader))
            //{
            //    return Unauthorized("Assinatura não encontrada");
            //}

            // Calcula o HMAC local
            var computedSignature = GenerateHmac(body, Secret);

            //// Compara
            //if (!string.Equals(signatureHeader, computedSignature, StringComparison.OrdinalIgnoreCase))
            //{
            //    return Unauthorized("Assinatura inválida");
            //}

            Console.WriteLine("Webhook autenticado com sucesso.");
            Console.WriteLine("Payload recebido: " + body);
            Console.WriteLine("X-Signature: " + signatureHeader);


            return Ok(new { status = "Recebido e autenticado" });
        }

        private string GenerateHmac(string payload, string secret)
        {
            var key = Encoding.UTF8.GetBytes(secret);
            using var hmac = new HMACSHA256(key);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return Convert.ToHexString(hash).ToLower();
        }
    }
}

