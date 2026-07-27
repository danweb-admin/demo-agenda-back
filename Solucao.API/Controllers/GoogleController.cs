using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Exceptions.Calendar;
using Solucao.Application.Exceptions.Model;
using System.Text.Json;
using Solucao.Application.Service.Interfaces;
using Solucao.Application.Exceptions.DigitalSignature;
using Solucao.Application.Exceptions.Integration;

namespace Solucao.API.Controllers
{
  [Route("api/v1/google")]
  [ApiController]
  public class GoogleController : ControllerBase
  {
    private readonly IGoogleService googleService;

    public GoogleController(IGoogleService _googleService)
    {
      googleService = _googleService;
    }

    [HttpPost()]
    public async Task<IActionResult> PostAsync([FromBody] GoogleRequest model)
    {
      try
      {
          Console.WriteLine(".....GOOGLE REQUEST.......");
          var json = JsonSerializer.Serialize(model, new JsonSerializerOptions
          {
            WriteIndented = true
          });

          Console.WriteLine(json);

          await googleService.ExtrairInformacoe(model);

          return Ok();
      }
      catch (IntegrationException ie)
      {
        return NotFound(ie.Message);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex);
      }
        

    }
  }
}

