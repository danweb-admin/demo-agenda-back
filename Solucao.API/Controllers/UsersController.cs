using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Solucao.Application.Utils;
using Solucao.Application.Contracts;
using Solucao.Application.Data.Entities;
using Solucao.Application.Service.Implementations;
using Solucao.Application.Service.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Solucao.Application.Contracts.Requests;

namespace Solucao.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly TokenService tokenService;
        private readonly ILogger<UsersController> logger;

        
        public UsersController(IUserService _userService, TokenService _tokenService, ILogger<UsersController> _logger)
        {
            userService = _userService;
            tokenService = _tokenService;
            logger = _logger;
        }


        [HttpGet("user")]
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(User))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApplicationError))]
        public async Task<IEnumerable<UserViewModel>> GetAllAsync(bool isDriver)
        {
            logger.LogInformation($"{nameof(GetAllAsync)} | Inicio da chamada");
            return await userService.GetAll(isDriver);
        }

        [HttpPost("user")]
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApplicationError))]
        public async Task<IActionResult> PostAsync([FromBody] User model)
        {
            var userAuthenticated = await userService.GetByName(User.Identity.Name);

            var result = await userService.Add(model,userAuthenticated.Id);

            if (result != null)
            {
                logger.LogWarning($"{nameof(PostAsync)} | Erro na criacao - {model.Email} - {result}");
                return NotFound(result);
            }
            return Ok(result);
        }


        [HttpPut("user/{id}")]
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApplicationError))]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] User model)
        {
            var userAuthenticated = await userService.GetByName(User.Identity.Name);

            var result = await userService.Update(model, id, userAuthenticated.Id);

            if (result != null)
            {
                logger.LogWarning($"{nameof(PutAsync)} | Erro na atualizacao - {model.Email} - {result}");
                return NotFound(result);

            }
            return Ok(result);
            
        }

        [HttpPost("user/change-user-password")]
        [AllowAnonymous]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ValidationResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApplicationError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApplicationError))]
        public async Task<IActionResult> ChangeUserPassworAsync([FromBody] ChangeUserPasswordRequest model)
        {
            logger.LogInformation($"{nameof(ChangeUserPassworAsync)} | Inicio da chamada - {model.Email}");
            // Recupera o usuário

            var userAuthenticated = await userService.GetByName(User.Identity.Name);

            var user = await userService.GetByEmail(model.Email);

            if (userAuthenticated.Name != user.Name)
            {
                logger.LogWarning($"{nameof(ChangeUserPassworAsync)} | Erro Autenticacao - {model.Email}");
                return BadRequest(new ApplicationError { Code = "404", Message = "Você não pode alterar a senha de outro usuário." });
            }

            if (user == null)
            {
                logger.LogWarning($"{nameof(ChangeUserPassworAsync)} | Erro Autenticacao - {model.Email}");
                return BadRequest(new ApplicationError { Code = "404", Message = "Usuário não encontrado." });

            }

            return Ok(await userService.ChangeUserPassword(user,model.Password, userAuthenticated.Id));
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] User model)
        {
            logger.LogInformation($"{nameof(Authenticate)} | Inicio da chamada - {model.Email}");
            // Recupera o usuário
            var user = await userService.Authenticate(model.Email, model.Password);

            // Verifica se o usuário existe
            if (user == null)
            {
                logger.LogWarning($"{nameof(Authenticate)} | Erro Autenticacao - {model.Email}");
                return BadRequest(new ApplicationError { Code = "400", Message = "Senha ou usuário inválido." });

            }

            // Gera o Token
            var token = tokenService.GenerateToken(user);

            // Retorna os dados
            return Ok(new
            {
                user = user,
                token = token
            });
        }
    }
}
