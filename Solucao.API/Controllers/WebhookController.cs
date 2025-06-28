using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Solucao.Application.Service.Interfaces;
using System;
using Microsoft.AspNetCore.Http;

namespace Solucao.API.Controllers
{
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly IDigitalSignatureService service;

        public WebhookController(IDigitalSignatureService _service)
        {
            service = _service;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {

            try
            {
                // Lê o corpo bruto da requisição
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                // Pega o HMAC enviado no header
                var signatureHeader = Request.Headers;

                var receivedHmac = Request.Headers["HMAC"].ToString();

                if (string.IsNullOrEmpty(receivedHmac))
                    return Unauthorized("Assinatura não encontrada");

                var result = await service.EventosWebhook(body);


                return Ok(new { status = "Processo atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        
    }
}

