using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Humanizer.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Response;
using Solucao.Application.Exceptions.DigitalSignature;
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
            catch (DigitalSignatureException dse)
            {
                return NotFound(dse.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            
        }

        [HttpGet("historico")]
        public async Task<IActionResult> History(Guid calendarId)
        {
            var result = await service.HistoricoAssinatura(calendarId);
    
            return Ok(result);
        }

    }
}

