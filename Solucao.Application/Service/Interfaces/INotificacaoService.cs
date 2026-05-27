using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Solucao.Application.Contracts;

public interface INotificacaoService
{
    Task<ValidationResult> Add(NotificacaoViewModel notificacao);
    Task<IEnumerable<NotificacaoViewModel>> GetAll();
    Task<IEnumerable<NotificacaoViewModel>> GetPendentes();
    Task<NotificacaoViewModel> GetById(Guid id);
    Task<NotificacaoViewModel> GetByToken(string token);
    Task<ValidationResult> Update(NotificacaoViewModel notificacao);

    Task<bool> ExistePorLocacao(Guid locacaoId);
    Task Remover(Guid locacaoId);

    Task MarcarComoEnviado(Guid id);
    Task MarcarComoFalha(Guid id);
    Task RegistrarResposta(string token, string resposta);
    Task EnviarWhatsapp(NotificacaoViewModel notificacao);
}