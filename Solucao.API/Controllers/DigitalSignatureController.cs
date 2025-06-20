using System;
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
        [AllowAnonymous]
        public async Task<IActionResult> PostAsync([FromBody] DigitalSignatureResponse model)
        {
            Console.WriteLine("**********************");
            Console.WriteLine(model?.evento);
            Console.WriteLine(model?.idProcesso);
            return Ok();
        }
    }
}

