  using System;
  using System.IO;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;
  using Solucao.Application.Contracts;
  using Solucao.Application.Contracts.Requests;
  using Solucao.Application.Exceptions.Calendar;
  using Solucao.Application.Exceptions.Model;
  using Solucao.Application.Service.Interfaces;

  namespace Solucao.API.Controllers
  {
      [Route("api/v1/generate-contract")]
      [ApiController]
      //[Authorize]
      public class GenerateContractController : ControllerBase
	  {
          private readonly IGenerateContractService service;

          public GenerateContractController(IGenerateContractService _service)
		  {
              service = _service;
		  }

          [HttpGet]
          public async Task<IActionResult> Get(DateTime? date)
          {
              if (!date.HasValue)
                  return BadRequest("Informe a data para buscar as locações");

              var contracts = await service.GetAllByDayAndContractMade(date.Value);
              return Ok(contracts);
          }

          [HttpPost()]
          public async Task<IActionResult> PostAsync([FromBody] GenerateContractRequest model)
          {
              try
              {
                  var result = await service.GenerateContract(model, null);

                  if (result != null)
                      return NotFound(result);
                  return Ok(result);

              }
              catch (ModelNotFoundException ex)
              {
                  return BadRequest(ex.Message);
              }
              catch (CalendarNoValueException ex)
              {
                  return BadRequest(ex.Message);
              }
              catch (Exception ex)
              {
                  return StatusCode(StatusCodes.Status500InternalServerError, ex);
              }

          }

          [HttpGet("multiple-contratos")]
          public async Task<IActionResult> MultipleAsync([FromQuery] string ids)
          {
              try
              {
                  var result = await service.GenerateMultipleContract(ids);

                  if (result != null)
                      return NotFound(result);
                  return Ok(result);

              }
              catch (ModelNotFoundException ex)
              {
                  return BadRequest(ex.Message);
              }
              catch (CalendarNoValueException ex)
              {
                  return BadRequest(ex.Message);
              }
              catch (Exception ex)
              {
                  return StatusCode(StatusCodes.Status500InternalServerError, ex);
              }

          }

          [HttpGet("download-contract")]
          public async Task<IActionResult> DownloadContract([FromQuery] Guid? id)
          {

              if (!id.HasValue)
                  return BadRequest("É preciso fornecer uma locação!");

              var result = await service.DownloadContract(id.Value);

              return File(result, "application/octet-stream", $"{id.Value}.docx");

          }


          [HttpGet("busca-locacoes")]
          public async Task<IActionResult> BuscaLocacoes( Guid cliendId, Guid equipmentId, DateTime startDate, DateTime endDate)
          {

              var result = await service.BuscarLocacoes(cliendId, equipmentId,startDate,endDate);

              return Ok(result);

          }
      }
  }

