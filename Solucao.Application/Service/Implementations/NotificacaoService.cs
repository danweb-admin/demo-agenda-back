using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using NetDevPack.Messaging;
using Newtonsoft.Json;
using Solucao.Application.Contracts;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Service.Interfaces;

namespace Solucao.Application.Service.Implementations
{
    public class NotificacaoService : INotificacaoService
    {
        private readonly NotificacaoRepository notificacaoRepository;
        private readonly IMapper mapper;

        public NotificacaoService(NotificacaoRepository _notificacaoRepository, IMapper _mapper)
        {
            notificacaoRepository = _notificacaoRepository;
            mapper = _mapper;
        }

        // ➕ Criar notificação (ainda não enviada)
        public async Task<ValidationResult> Add(NotificacaoViewModel notificacao)
        {
            notificacao.Id = Guid.NewGuid();
            notificacao.CreatedAt = DateTime.Now;
            notificacao.Status = "P"; // Pendente
            notificacao.Active = true;

            var _notificacao = mapper.Map<Notificacao>(notificacao);

            await notificacaoRepository.Add(_notificacao);

            return ValidationResult.Success;
        }

        // 🔍 Listar todas (central de notificações)
        public async Task<IEnumerable<NotificacaoViewModel>> GetAll()
        {
            return mapper.Map<IEnumerable<NotificacaoViewModel>>(await notificacaoRepository.GetAll());
        }

        // 🔍 Buscar por Id
        public async Task<NotificacaoViewModel> GetById(Guid id)
        {
            var notificacao = await notificacaoRepository.GetById(id);
            return mapper.Map<NotificacaoViewModel>(notificacao);
        }

        // 🔍 Buscar por token (usado na confirmação)
        public async Task<NotificacaoViewModel> GetByToken(string token)
        {
            var notificacao = await notificacaoRepository.GetByToken(token);
            return mapper.Map<NotificacaoViewModel>(notificacao);
        }

        // ✏️ Atualizar
        public async Task<ValidationResult> Update(NotificacaoViewModel notificacao)
        {
            var _notificacao = mapper.Map<Notificacao>(notificacao);

            await notificacaoRepository.Update(_notificacao);

            return ValidationResult.Success;
        }

        // 📤 Marcar como enviado
        public async Task MarcarComoEnviado(Guid id)
        {
            var notificacao = await notificacaoRepository.GetById(id);

            if (notificacao == null) return;

            notificacao.Status = 'E';
            notificacao.DataEnvio = DateTime.Now;
            notificacao.UpdatedAt = DateTime.Now;

            await notificacaoRepository.Update(notificacao);
        }

        // ❌ Marcar como falha
        public async Task MarcarComoFalha(Guid id)
        {
            var notificacao = await notificacaoRepository.GetById(id);

            if (notificacao == null) return;

            notificacao.Status = 'F';
            notificacao.Tentativas += 1;
            notificacao.UpdatedAt = DateTime.Now;

            await notificacaoRepository.Update(notificacao);
        }

        // ✅ Registrar resposta (confirmação/cancelamento)
        public async Task RegistrarResposta(string token, string resposta)
        {
            await notificacaoRepository.RegistrarResposta(token, resposta);
        }

        public async Task<bool> ExistePorLocacao(Guid locacaoId)
        {
          return await notificacaoRepository.ExistePorLocacao(locacaoId);
        }

        public async Task EnviarWhatsapp(NotificacaoViewModel notificacao)
        {
            var evolutionAPI = Environment.GetEnvironmentVariable("EvolutionAPI");
            var apiKeyEvolutionApi = Environment.GetEnvironmentVariable("ApiKeyEvolutionApi");


            if (string.IsNullOrEmpty(evolutionAPI))
            {
                Console.WriteLine($"❌ Parametro Evolution API não informado.");
                return;
            }

            if (string.IsNullOrEmpty(apiKeyEvolutionApi))
            {
                Console.WriteLine($"❌ Parametro ApiKey Evolution API não informado.");
                return;
            }

            using var client = new HttpClient();

            

            var body = new
            {
                number = "5543999510994",
                textMessage = new
                {
                    text = notificacao.Mensagem
                }
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{evolutionAPI}/message/sendText/danweb"
            );

            request.Headers.Add("apikey", apiKeyEvolutionApi);

            request.Content = new StringContent(
                JsonConvert.SerializeObject(body),
                Encoding.UTF8,
                "application/json");

      
            var response = await client.SendAsync(request);

            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine(result);
            
        }

        public async Task<IEnumerable<NotificacaoViewModel>> GetPendentes()
        {
          return mapper.Map<IEnumerable<NotificacaoViewModel>>(await notificacaoRepository.GetPendentes());
 
        }

        public async Task Remover(Guid locacaoId)
        {
          await notificacaoRepository.Remover(locacaoId);
        }
    }
}