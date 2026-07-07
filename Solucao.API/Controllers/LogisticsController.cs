using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solucao.Application.Contracts;
using Solucao.Application.Service.Implementations;
using Solucao.Application.Service.Interfaces;
using Solucao.Application.Utils;

namespace Solucao.API.Controllers
{
    [Route("api/v1/logistics")]
    [ApiController]
    [Authorize]
    public class LogisticsController : ControllerBase
    {
        private readonly ILogisticsService service;
        private readonly IUserService userService;


        public LogisticsController(ILogisticsService service, IUserService userService)
        {
            this.service = service;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var logistics = await service.GetAll();
            return Ok(logistics);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var logistics = await service.GetById(id);

            if (logistics == null)
                return NotFound();

            return Ok(logistics);
        }

        [HttpGet("date")]
        public async Task<IActionResult> GetByDate(DateTime data)
        {
            var logistics = await service.GetByDate(data);
            return Ok(logistics);
        }

        [HttpGet("driver/{driverId}")]
        public async Task<IActionResult> GetByDriver(Guid driverId, DateTime data)
        {
            var logistics = await service.GetByDriver(driverId, data);
            return Ok(logistics);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LogisticsViewModel logistics)
        {
            try
            {
                var user = await userService.GetByName(User.Identity.Name);

                var result = await service.Add(logistics, user.Id);

                if (result != ValidationResult.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] LogisticsViewModel logistics)
        {
            try
            {
                logistics.Id = id;

                var user = await userService.GetByName(User.Identity.Name);

                var result = await service.Update(logistics, user.Id);

                if (result != ValidationResult.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
          try
            {
                var user = await userService.GetByName(User.Identity.Name);

                var result = await service.Remove(id, user.Id);

                if (result != ValidationResult.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete(Guid id)
        {
            try
            {
                var user = await userService.GetByName(User.Identity.Name);

                var result = await service.Complete(id, user.Id);

                if (result != ValidationResult.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/uncomplete")]
        public async Task<IActionResult> Uncomplete(Guid id)
        {
            try
            {
                var user = await userService.GetByName(User.Identity.Name);

                var result = await service.Uncomplete(id, user.Id);

                if (result != ValidationResult.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}