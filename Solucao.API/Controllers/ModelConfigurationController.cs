using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Service.Implementations;
using Solucao.Application.Service.Interfaces;

namespace Solucao.API.Controllers
{
    [Route("api/v1/model-configuration")]
    [ApiController]
    [Authorize]
    public class ModelConfigurationController : ControllerBase
	{
        private readonly IModelConfigurationService service;

        public ModelConfigurationController(IModelConfigurationService _service)
		{
            service = _service;
        }

        [HttpGet("attribute-type")]
        public async Task<IActionResult> GetAttributeTypes()
        {
            var modelos = await service.GetAttributeTypes();
            return Ok(modelos);
        }

        [HttpGet("techinical-attributes")]
        public async Task<IActionResult> GetTechnicalAttributes()
        {
            var modelos = await service.GetTechnicalAttributes();
            return Ok(modelos);
        }
    }
}

