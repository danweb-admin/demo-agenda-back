using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Contracts.Response;
using Solucao.Application.Data.Entities;
using Solucao.Application.Service.Interfaces;
using Solucao.Application.Utils;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Solucao.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    //[Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService clientService;

        public ClientsController(IClientService _clientService)
        {
            clientService = _clientService;
        }

        [HttpGet("client/value-by-equipament")]
        public async Task<decimal> GetValueByEquipament([FromQuery] Guid clientId, Guid equipamentId, string startTime, string endTime)
        {
            return await clientService.GetValueByEquipament(clientId,equipamentId,startTime,endTime);
        }

        [HttpGet("client")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Client))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApplicationError))]
        public async Task<IEnumerable<ClientViewModel>> GetAllAsync([FromQuery] ClientRequest clientRequest)
        {
            return await clientService.GetAll(clientRequest.Ativo, clientRequest.Search);
        }

        [HttpGet("client/by-id")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Client))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApplicationError))]
        public async Task<ClientViewModel> GetByIdAsync([FromQuery] Guid? id)
        {
            return await clientService.GetById(id);
        }


        [HttpPost("client")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApplicationError))]
        public async Task<IActionResult> PostAsync([FromBody] ClientViewModel model)
        {
            var result = await clientService.Add(model);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }


        [HttpPut("client/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApplicationError))]
        public async Task<IActionResult> PutAsync(string id, [FromBody] ClientViewModel model)
        {
            var result = await clientService.Update(model);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("client/adjust-equipment-values")]
        public async Task AdjustEquipmentValues()
        {
             await clientService.AdjustEquipmentValues();
        }

        [HttpGet("client/migrate-client_values")]
        public async Task MigrateClientValues()
        {
            await clientService.MigrateClientValues();
        }

        [HttpGet("client/client-equipment")]
        public async Task<IEnumerable<ClientEquipmentNamesViewModel>> ClientEquipmentValues(string clientName)
        {
            return await clientService.ClientEquipment(clientName);
        }

        [HttpPut("client/client-equipment")]
        public async Task<IActionResult> ClientEquipmentSave(ClientEquipmentNamesViewModel viewModel)
        {
            var result = await clientService.ClientEquipmentSave(viewModel);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }


    }
}
