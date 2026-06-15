using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Solucao.Application.Contracts;
using Solucao.Application.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Solucao.Application.Contracts.Requests;

namespace Solucao.API.Controllers
{
    [Route("api/v1/notificacao")]
    [ApiController]
    [Authorize]
    public class NotificacaoController : ControllerBase
    {
        private readonly INotificacaoService notificacaoService;

        public NotificacaoController(INotificacaoService _notificacaoService)
        {
            notificacaoService = _notificacaoService;
        }

        // 🔍 Listar todas (central de notificações)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await notificacaoService.GetAll();
            return Ok(result);
        }

        // 🔍 Buscar por Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await notificacaoService.GetById(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // ➕ Criar notificação
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificacaoViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await notificacaoService.Add(model);

            if (result != System.ComponentModel.DataAnnotations.ValidationResult.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // ✏️ Atualizar
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] NotificacaoViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await notificacaoService.Update(model);

            if (result != System.ComponentModel.DataAnnotations.ValidationResult.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // 🔗 Buscar por token (usado na tela de confirmação)
        [HttpGet("token/{token}")]
        public async Task<IActionResult> GetByToken(string token)
        {
            var result = await notificacaoService.GetByToken(token);

            if (result == null)
                return NotFound("Link inválido ou expirado");

            return Ok(result);
        }

        // ✅ Confirmar / Cancelar locação
        [HttpPost("responder")]
        public async Task<IActionResult> Responder([FromBody] ResponderNotificacaoRequest request)
        {
            if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.Resposta))
                return BadRequest("Token e resposta são obrigatórios");

            await notificacaoService.RegistrarResposta(request.Token, request.Resposta);

            return Ok(new
            {
                mensagem = "Resposta registrada com sucesso"
            });
        }
    }
}