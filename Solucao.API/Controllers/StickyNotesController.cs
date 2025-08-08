using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
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
    [Authorize]
    public class StickyNotesController : ControllerBase
    {
        private IStickyNoteService stickyNoteService;
        private readonly IUserService userService;
        public StickyNotesController(IStickyNoteService _stickyNoteService, IUserService _userService)
        {
            stickyNoteService = _stickyNoteService;
            userService = _userService;
        }
        [HttpGet("sticky-notes")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(StickyNote))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApplicationError))]
        public async Task<IEnumerable<StickyNoteViewModel>> GetAllAsync([FromQuery] StickyNotesRequest model)
        {
            return await stickyNoteService.GetAll(model.Date);
        }

        [HttpPost("sticky-notes")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApplicationError))]
        public async Task<IActionResult> PostAsync([FromBody] StickyNoteViewModel model)
        {
            var user = await userService.GetByName(User.Identity.Name);
            model.UserId = user.Id;

            var result = await stickyNoteService.Add(model);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }


        [HttpPut("sticky-notes/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApplicationError))]
        public async Task<IActionResult> PutAsync(string id, [FromBody] StickyNoteViewModel model)
        {
            var user = await userService.GetByName(User.Identity.Name);
            model.UserId = user.Id;

            var result = await stickyNoteService.Update(model);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPut("sticky-notes/update-resolved/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApplicationError))]
        public async Task<IActionResult> UpdateResolvedAsync(Guid id)
        {
            var result = await stickyNoteService.UpdateResolved(id);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPut("sticky-notes/remove/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApplicationError))]
        public async Task<IActionResult> RemoveAsync(Guid id)
        {
            var result = await stickyNoteService.Remove(id);

            if (result != null)
                return NotFound(result);
            return Ok(result);
        }
    }
}
