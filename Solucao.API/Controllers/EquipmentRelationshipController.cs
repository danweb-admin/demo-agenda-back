using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Service.Interfaces;

namespace Solucao.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class EquipmentRelationshipController : ControllerBase
    {
        private readonly IEquipmentRelantionshipService service;

        public EquipmentRelationshipController(IEquipmentRelantionshipService _service)
        {
            service = _service;
        }

        [HttpGet("equipment-relationship")]
        public async Task<IEnumerable<EquipmentRelationshipViewModel>> GetAllAsync([FromQuery] EquipamentRequest request)
        {
            return await service.GetAll(request.Ativo);
        }

        [HttpPost("equipment-relationship")]
        public async Task<IActionResult> PostAsync([FromBody] EquipmentRelationshipViewModel model)
        {

            var result = await service.Add(model);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPut("equipment-relationship/{id}")]
        public async Task<IActionResult> PutAsync(string id, [FromBody] EquipmentRelationshipViewModel model)
        {
            var result = await service.Update(model);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }
    }
}

