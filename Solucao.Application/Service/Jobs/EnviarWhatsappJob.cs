using System;
using System.Threading.Tasks;
using Solucao.Application.Data.Repositories;

namespace Solucao.Application.Service.Jobs
{
  public class EnviarWhatsappJob
  {
    private readonly INotificacaoService notificacaoService;

    public EnviarWhatsappJob(INotificacaoService _notificacaoService)
    {
        notificacaoService = _notificacaoService;
    }
    
    public async Task Executar()
    {
        var pendentes = await notificacaoService.GetPendentes();

        foreach(var notificacao in pendentes)
        {
            try
            {
                await notificacaoService.EnviarWhatsapp(notificacao);

                notificacao.Status = "E";
                notificacao.DataEnvio = DateTime.Now;
            }
            catch
            {
                notificacao.Tentativas++;

                notificacao.Status = "F";
            }

            await notificacaoService.Update(notificacao);

            await Task.Delay(3000);
        }
    }
  }
}

