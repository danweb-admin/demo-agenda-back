using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Service.Implementations;
using Solucao.Application.Service.Interfaces;
using Solucao.Application.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace Solucao.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class PriceTableController : ControllerBase
    {
        private readonly IPriceTableService priceTableService;

        public PriceTableController(IPriceTableService _priceTableService)
        {
            priceTableService = _priceTableService;
        }


        [HttpGet("price-table")]
        public async Task<IActionResult> GetAllAsync([FromQuery] PriceTableRequest priceTableRequest)
        {
            var result = await priceTableService.GetAll(priceTableRequest.EquipmentId);

            return Ok(result);
        }



        

        [HttpPost("price-table")]
        public async Task<IActionResult> PostAsync([FromBody] List<PriceTableValuesRequest> model)
        {
            var result = await priceTableService.Save(model);

            if (result != null)
                return NotFound(result);
            return Ok();
        }

        [HttpGet("price-table/value-by-equipment")]
        public async Task<IActionResult> ValueBy([FromQuery] PriceTableRequest model)
        {
            var result = await priceTableService.ValueByEquipment(model);

            return Ok(result);
        }


        [HttpPut("price-table/{id}")]
        public async Task<IActionResult> PutAsync(string id, [FromBody] PriceTableViewModel model)
        {
            var result = await priceTableService.Update(model);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }
    }
}

