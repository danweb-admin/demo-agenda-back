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
            var link = $"https://swr-locacoes-agenda.online/confirmarcao?token={token}";
            var hora = locacao.StartTime.Value.ToString("HH:mm") + " a " + locacao.EndTime.Value.ToString("HH:mm"); 

            var mensagem = $@"
              Olá, *{nome}*! 😊

              Estamos passando para confirmar sua locação do equipamento *{equipamento}*.

              📅 Data: *{data:dd/MM/yyyy}*  
              ⏰ Horário: *{hora}*

              Por favor, confirme:
              👉 {link}
              ";
              

              var notificacao = new NotificacaoViewModel
              {
                  LocacaoId = locacao.Id,
                  Telefone = telefoneFormatado, // ajusta conforme seu model
                  Mensagem = mensagem,
                  TokenConfirmacao = token
              };

              await notificacaoService.Add(notificacao);
          }
      }
  }
}

