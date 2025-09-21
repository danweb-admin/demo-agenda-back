using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solucao.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class SpecificationsController : ControllerBase
    {
        private readonly ISpecificationService specificationService;

        public SpecificationsController(ISpecificationService _spescificationService)
        {
            specificationService = _spescificationService;
        }

        [HttpGet("specifications")]
        [AllowAnonymous]
        public async Task<IEnumerable<SpecificationViewModel>> GetAllAsync()
        {
            return await specificationService.GetAll();
        }

        [HttpGet("specifications/get-specification-by-equipament")]
        public async Task<IEnumerable<SpecificationViewModel>> GetSpecficationByEquipamentAsync([FromQuery] SpecificationRequest model)
        {
            List<Guid> list = new List<Guid>();
            if (!string.IsNullOrEmpty(model.EquipamentList))
                list = model.EquipamentList.Split(',').Select(Guid.Parse).ToList();
            return await specificationService.GetSpecificationByEquipament(list);
        }

        [HttpPost("specifications")]
        public async Task<IActionResult> PostAsync([FromBody] SpecificationViewModel model)
        {
            var result = await specificationService.Add(model);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }


        [HttpPut("specifications/{id}")]
        public async Task<IActionResult> PutAsync(string id, [FromBody] SpecificationViewModel model)
        {
            var result = await specificationService.Update(model);

            if (result != null)
                return NotFound(result);
            return Ok(result);
            
        }
    }
}
