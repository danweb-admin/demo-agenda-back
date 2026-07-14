using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Exceptions.Calendar;
using Solucao.Application.Exceptions.Model;
using System.Text.Json;
using Solucao.Application.Service.Interfaces;

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
        Console.WriteLine(".....GOOGLE REQUEST.......");
        var json = JsonSerializer.Serialize(model, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Console.WriteLine(json);

        await googleService.ExtrairInformacoe(model);

        return Ok();

    }
  }
}

