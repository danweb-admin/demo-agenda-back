using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Response;
using Solucao.Application.Service.Interfaces;

namespace Solucao.API.Controllers
{
    [Route("api/v1/assinatura-digital")]
    [ApiController]
    //[Authorize]
    public class DigitalSignatureController : ControllerBase
    {
        private readonly IDigitalSignatureService service;

        public DigitalSignatureController(IDigitalSignatureService _service)
		{
            service = _service;
		}

        [HttpGet]
        public async Task<IActionResult> Get(Guid calendarId)
        {
            try
            {
                var result = await service.EnviarDocumentoParaAssinar(calendarId);

                if (result != null)
                    return NotFound(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            
        }

        [HttpPost("webhook-response")]
        public async Task<IActionResult> PostAsync([FromBody] DigitalSignatureResponse response)
        {
            try
            {
                var result = await service.EventosWebhook(response);

                if (result != null)
                    return NotFound(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost]
        [HttpPost("webhook-response2")]
        public async Task<IActionResult> Receive([FromBody] JsonElement payload)
        {
            // Você pode processar o payload aqui
            Console.WriteLine("Webhook recebido: " + payload.ToString());

            // Exemplo: logar em arquivo
            //await System.IO.File.AppendAllTextAsync("webhook_log.txt", payload.ToString() + Environment.NewLine);

            return Ok(new { status = "success", receivedAt = DateTime.UtcNow });
        }

        [HttpPut]
        [HttpPost("webhook-response-put")]
        public async Task<IActionResult> ReceivePut([FromBody] JsonElement payload)
        {
            // Você pode processar o payload aqui
            Console.WriteLine("Webhook recebido put: " + payload.ToString());

            // Exemplo: logar em arquivo
            //await System.IO.File.AppendAllTextAsync("webhook_log.txt", payload.ToString() + Environment.NewLine);

            return Ok(new { status = "success", receivedAt = DateTime.UtcNow });
        }

        [HttpPatch]
        [HttpPost("webhook-response-patch")]
        public async Task<IActionResult> ReceivePatch([FromBody] JsonElement payload)
        {
            // Você pode processar o payload aqui
            Console.WriteLine("Webhook recebido patch: " + payload.ToString());

            // Exemplo: logar em arquivo
            //await System.IO.File.AppendAllTextAsync("webhook_log.txt", payload.ToString() + Environment.NewLine);

            return Ok(new { status = "success", receivedAt = DateTime.UtcNow });
        }
    }
}

