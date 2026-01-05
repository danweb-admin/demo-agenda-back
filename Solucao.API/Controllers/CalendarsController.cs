using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class CalendarsController : ControllerBase
    {
        private readonly ICalendarService calendarService;
        private readonly IUserService userService;
        private readonly ILogger<CalendarsController> logger;

        public CalendarsController(ICalendarService _calendarService, IUserService _userService, ILogger<CalendarsController> _logger)
        {
            calendarService = _calendarService;
            userService = _userService;
            logger = _logger;
        }

        [HttpGet("calendar/get-all")]
        public async Task<IEnumerable<CalendarViewModel>> GetAllAsync([FromQuery] CalendarRequest model)
        {
            
            return await calendarService.GetAll(model.Date);
        }

        [HttpGet("calendar")]
        public async Task<IEnumerable<EquipamentList>> GetAllByDateAsync([FromQuery] CalendarRequest model)
        {
            logger.LogInformation($"{DateTime.Now} - {nameof(CalendarsController)} -{nameof(GetAllAsync)} | Inicio da chamada");
            return await calendarService.GetAllByDate(model.Date);
        }

        [HttpGet("calendar/by-id")]
        [AllowAnonymous]
        public async Task<CalendarViewModel> GetByIdAsync([FromQuery] Guid id)
        {
            logger.LogInformation($"{nameof(CalendarsController)} -{nameof(GetAllAsync)} | Inicio da chamada");
            return await calendarService.GetById(id);
        }

        [HttpGet("calendar/schedules")]
        public async Task<IEnumerable<CalendarViewModel>> SchedulesAsync([FromQuery] CalendarRequest model)
        {
            logger.LogInformation($"{nameof(CalendarsController)} -{nameof(SchedulesAsync)} | Inicio da chamada");
            var list = new List<Guid>();
            var equipamentIds = new List<Guid>();

            if (!string.IsNullOrEmpty(model.DriverList))
                list = model.DriverList.Split(',').Select(Guid.Parse).ToList();

            if (!string.IsNullOrEmpty(model.EquipamentList))
                equipamentIds = model.EquipamentList.Split(',').Select(Guid.Parse).ToList();


            return await calendarService.Schedules(model.StartDate, model.EndDate, model.ClientId, equipamentIds, list, model.TechniqueId, model.Status);
        }

        [HttpGet("calendar/availability")]
        public async Task<string> AvailabilityAsync([FromQuery] CalendarRequest model)
        {
            logger.LogInformation($"{nameof(CalendarsController)} -{nameof(AvailabilityAsync)} | Inicio da chamada");

            var equipamentIds = new List<Guid>();
            var specificationIds = new List<Guid>();
            if (!string.IsNullOrEmpty(model.EquipamentList))
                equipamentIds = model.EquipamentList.Split(',').Select(Guid.Parse).ToList();

            return await calendarService.Availability(equipamentIds, model.Month, model.Year);
        }

        [HttpPost("calendar")]
        [AllowAnonymous]
        public async Task<IActionResult> PostAsync([FromBody] CalendarViewModel model)
        {
            logger.LogInformation($"{nameof(CalendarsController)} - {nameof(PostAsync)} | Inicio da chamada");
            ValidationResult result;
            result = await calendarService.ValidateLease(model.Date, model.ClientId, model.EquipamentId, model.CalendarSpecifications, model.StartTime1, model.EndTime1);

            if (result != null)
            {
                logger.LogWarning($"{nameof(CalendarsController)} -{nameof(PostAsync)} | Erro na criacao - {result}");
                if (!result.ErrorMessage.Contains("minutos"))
                    return NotFound(result);
                else
                    model.Note += result.ErrorMessage;
            }
            
            var user = await userService.GetByName(User.Identity.Name);

            if (user == null)
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(token))
                    return NotFound("Token não fornecido. Entre em contato com o suporte.");

                user = await userService.GetByToken(token.Replace("Bearer ", ""));

                if (user == null)
                    return NotFound("Você não tem permissão para visualizar os dados dessa página. Entre em contato com o suporte.");

                var hoje = DateTime.Now;

                if (hoje.Date > user.Token_Expire.Value.Date)
                    return NotFound("Token expirado. Entre em contato com o suporte.");
            }


            result = await calendarService.Add(model, user.Id);

            if (result != null)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("calendar/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> PutAsync([FromBody] CalendarViewModel model)
        {
            ValidationResult result;
            result = await calendarService.ValidateLease(model.Date, model.ClientId, model.EquipamentId, model.CalendarSpecifications, model.StartTime1, model.EndTime1);

            if (result != null)
            {
                if (!result.ErrorMessage.Contains("minutos"))
                    return NotFound(result);
                else
                    model.Note += result.ErrorMessage;
            }

            var user = await userService.GetByName(User.Identity.Name);

            

            result = await calendarService.Update(model, user.Id);

            if (result != null)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("calendar/agendamento/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> PutAgendamentoAsync([FromBody] CalendarViewModel model)
        {
            ValidationResult result;
            result = await calendarService.ValidateLease(model.Date, model.ClientId, model.EquipamentId, model.CalendarSpecifications, model.StartTime1, model.EndTime1);


            if (result != null)
            {
                if (!result.ErrorMessage.Contains("minutos"))
                    return NotFound(result);
                else
                    model.Note += result.ErrorMessage;
            }

            var user = await userService.GetByName(User.Identity.Name);
           
            if (user == null)
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(token))
                    return NotFound("Token não fornecido. Entre em contato com o suporte.");

                user = await userService.GetByToken(token.Replace("Bearer ", ""));

                if (user == null)
                    return NotFound("Você não tem permissão para visualizar os dados dessa página. Entre em contato com o suporte.");

                var hoje = DateTime.Now;

                if (hoje.Date > user.Token_Expire.Value.Date)
                    return NotFound("Token expirado. Entre em contato com o suporte.");
            }

            result = await calendarService.UpdateAgendamento(model, user.Id);

            if (result != null)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("calendar/update-driver-or-technique-calendar")]
        public async Task<IActionResult> UpdateDriverOrTechniqueCalendarAsync([FromBody] CalendarRequest model)
        {
            ValidationResult result;
            result = await calendarService.UpdateDriverOrTechniqueCalendar(model.CalendarId.Value, model.PersonId.Value, model.IsDriver, model.isCollect);

            if (result != null)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("calendar/update-status-or-travel-on-calendar")]
        public async Task<IActionResult> UpdateStatusOrTravelOnCalendarAsync([FromBody] CalendarRequest model)
        {
            ValidationResult result;
            result = await calendarService.UpdateStatusOrTravelOnCalendar(model.CalendarId.Value, model.Status, model.TravelOn, model.IsTravelOn);

            if (result != null)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("calendar/update-contract-made")]
        public async Task<IActionResult> UpdateContractMadeAsync([FromBody] CalendarRequest model)
        {
            ValidationResult result;
            result = await calendarService.UpdateContractMade(model.CalendarId.Value);

            if (result != null)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("calendar/bulk-scheduling")]
        public async Task<IActionResult> BulkSchedulingAsync([FromBody] BulkSchedulingRequest model)
        {
            var user = await userService.GetByName(User.Identity.Name);

            return Ok(await calendarService.BulkScheduling(model, user.Id));
        }

        [HttpGet("calendar/report")]
        public async Task<IActionResult> CalendarReport([FromQuery] CalendarReportRequest model)
        {
            
            return Ok(await calendarService.CalendarReport(model));
        }

        [HttpGet("calendar/view")]
        [AllowAnonymous]
        public async Task<IActionResult> CalendarView([FromQuery] DateTime startDate, DateTime endDate)
        {
            Console.WriteLine($"calendar/view -> {DateTime.UtcNow} -> Motorista");
            
            return Ok(await calendarService.CalendarView(startDate, endDate, true));
        }
    

        [HttpGet("calendar/view-agendamento")]
        [AllowAnonymous]
        public async Task<IActionResult> CalendarViewAgendamentos([FromQuery] DateTime startDate, DateTime endDate)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token))
                return NotFound("Token não fornecido. Entre em contato com o suporte.");

            var user = await userService.GetByToken(token.Replace("Bearer ", ""));

            if (user == null)
                return NotFound("Você não tem permissão para visualizar os dados dessa página. Entre em contato com o suporte.");

            var hoje = DateTime.Now;

            if (hoje.Date >  user.Token_Expire.Value.Date )
                return NotFound("Token expirado. Entre em contato com o suporte.");



            return Ok(await calendarService.CalendarView(startDate,endDate,true, user.Id    ));
        }
    }
}
