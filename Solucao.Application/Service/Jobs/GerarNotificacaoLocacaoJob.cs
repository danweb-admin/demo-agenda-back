using System;
using System.Threading.Tasks;
using Solucao.Application.Contracts;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Repositories;

namespace Solucao.Application.Service.Jobs
{
  public class GerarNotificacaoLocacaoJob
  {
      private readonly CalendarRepository calendarRepository;
      private readonly INotificacaoService notificacaoService;

      public GerarNotificacaoLocacaoJob(
          CalendarRepository _calendarRepository,
          INotificacaoService _notificacaoService)
      {
          calendarRepository = _calendarRepository;
          notificacaoService = _notificacaoService;
      }

      public async Task Executar()
      {
          var urlConfirmacao = Environment.GetEnvironmentVariable("UrlConfirmacao");
          var apiKeyEvolutionApi = Environment.GetEnvironmentVariable("ApiKeyEvolutionApi");
          var locadorName = Environment.GetEnvironmentVariable("LocadorName");

          if (string.IsNullOrEmpty(locadorName))
          {
              Console.WriteLine($"❌ Parametro Nome Locador não informado.");
              return;
          }

          if (string.IsNullOrEmpty(urlConfirmacao))
          {
              Console.WriteLine($"❌ Parametro URL Confirmaçao não informado.");
              return;
          }

          if (string.IsNullOrEmpty(apiKeyEvolutionApi))
          {
              Console.WriteLine($"❌ Parametro ApiKey Evolution API não informado.");
              return;
          }

          var locacoes = await calendarRepository.GetLocacoesPendentesAmanha();

          foreach (var locacao in locacoes)
          {
            var telefoneFormatado = string.Empty;
            var token = Guid.NewGuid().ToString();

            var existe = await notificacaoService.ExistePorLocacao(locacao.Id);
            
            if (existe)
            {
              Console.WriteLine($"❌ Locacao já existe: {locacao.Date.ToShortDateString()} - {locacao.StartTime.Value.ToShortTimeString()} - {locacao.EndTime.Value.ToShortTimeString()} - {locacao.Client.Name}, {locacao.Equipament.Name}");
              continue;
            }

            var telefoneFormat = Utils.Helpers.TryFormatarTelefone(locacao.Client.CellPhone,out telefoneFormatado);

            if (!telefoneFormat)
            {
              Console.WriteLine($"❌ Telefone Locatario inválido: {locacao.Date.ToShortDateString()} - {locacao.StartTime.Value.ToShortTimeString()} - {locacao.EndTime.Value.ToShortTimeString()} - {locacao.Client.Name}, {locacao.Equipament.Name}");
              continue;
            }

            var nome = locacao.Client.Name;
            var data = locacao.Date;
            var equipamento = locacao.Equipament.Name;
            var link = $"{urlConfirmacao}confirmacao?token={token}";
            var hora = locacao.StartTime.Value.ToString("HH:mm") + " a " + locacao.EndTime.Value.ToString("HH:mm"); 

var mensagem = 
$@"Olá, *{nome.Trim()}*! 😊

Somos da *{locadorName}* e gostaríamos de confirmar sua locação.

📦 *Equipamento:* {equipamento}
📅 *Data:* {data:dd/MM/yyyy}
⏰ *Horário:* {hora}

Por favor, clique no link abaixo para realizar a confirmação da locação:
👉 {link}";
              

              var notificacao = new NotificacaoViewModel
              {
                  LocacaoId = locacao.Id,
                  Telefone = telefoneFormatado, // ajusta conforme seu model
                  Mensagem = mensagem,
                  TokenConfirmacao = token,
                  Tentativas = 0
              };

              try
              {
                await notificacaoService.Add(notificacao);

              }
              catch (Exception ex)
              {
                Console.WriteLine(ex);
              }

          }
      }
  }
}

