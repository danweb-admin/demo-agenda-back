using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Interfaces;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Exceptions.Integration;
using Solucao.Application.Service.Interfaces;
using Calendar = Solucao.Application.Data.Entities.Calendar;

namespace Solucao.Application.Service.Implementations
{
  public class GoogleService : IGoogleService
  {
    private CalendarRepository calendarRepository;
    private ICalendarService calendarService;

    private IClientRepository clientRepository;
    private IEquipamentRepository equipmentRepository;
    private UserRepository userRepository;

    public GoogleService(CalendarRepository _calendarRepository, IClientRepository _clientRepository, IEquipamentRepository _equipmentRepository, UserRepository _userRepository, ICalendarService _calendarService)
    {
      calendarRepository = _calendarRepository;
      clientRepository = _clientRepository;
      equipmentRepository = _equipmentRepository;
      userRepository = _userRepository;
      calendarService = _calendarService;

    }

    public async Task ExtrairInformacoe(GoogleRequest request)
    {

      
        var locacao  = await calendarRepository.GetByGoogleEventId(request.Id);

        if (locacao == null)
          await InsereLocacao(request);
      
    }

    private async Task<bool> InsereLocacao(GoogleRequest request)
    {
        // Cliente e aparelho
        var aparelho = await equipmentRepository.GetByName(request.Aparelho.Trim());

        if (aparelho == null )
          throw new IntegrationException($"Aparelho: {request.Aparelho.Trim()}, Aparelho não encontrado.");

        var cliente = await clientRepository.GetByIntegrationName(request.Titulo);

        if (cliente == null)
          throw new IntegrationException($"Locatário: {request.Titulo.Trim()}, Locatário não encontrado.");

        var user = await userRepository.GetByEmail("admin@admin.com");

        var horaInicio =  DateTime.Parse(request.Inicio);
        var horaFim = DateTime.Parse(request.Fim);

        CalendarViewModel locacao = new CalendarViewModel
        {
          ClientId = cliente.Id,
          EquipamentId = aparelho.Id,
          Date = horaInicio,
          StartTime = horaInicio,
          EndTime = horaFim,
          GoogleEventId = request.Id,
          CreatedAt = DateTime.Now,
          UserId = user.Id,
          Active = true
        };

        ExtrairIntegracaoDescricao(ref locacao, request.Descricao);

      try
      {
        var result = await calendarService.Add(locacao,user.Id);

        if (result == null)
          return true;
      }
      catch (Exception ex)
      {
        throw;
      }

        

        return false;
      
 
    }

    private void ExtrairIntegracaoDescricao(ref CalendarViewModel locacao, string descricao)
    {
        var match = Regex.Match(
        descricao,
        @"#INTEGRACAO\s*(.*?)\s*#FIMINTEGRACAO",
        RegexOptions.Singleline | RegexOptions.IgnoreCase);

        if (!match.Success)
          throw new IntegrationException($"Integracao não encontrada.");
            

        string bloco = match.Groups[1].Value;
        bloco = Regex.Replace(bloco, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
        bloco = Regex.Replace(bloco, "<.*?>", string.Empty);
        bloco = bloco.Replace("&nbsp;", " ").Trim();

        var dados = new Dictionary<string, string>();

        foreach (var linha in bloco.Split('\n'))
        {
            if (string.IsNullOrWhiteSpace(linha))
                continue;

            var partes = linha.Split('=', 2);

            if (partes.Length == 2)
            {
                dados[partes[0].Trim().ToUpper()] =
                    partes[1].Trim();
            }
        }

        var status = dados.TryGetValue("STATUS", out var s) ? s : "pendente";

        var valorLocacao = ObterDecimal(dados, "LOCACAO");
        var frete = ObterDecimal(dados, "FRETE");
        var desconto = ObterDecimal(dados, "DESCONTO");

        locacao.Status = StatusToString(status);
        locacao.Freight = frete;
        locacao.Discount = desconto;
        locacao.Value = valorLocacao;
        locacao.TotalValue = valorLocacao + frete - desconto;

        locacao.GoogleNote =  Regex.Replace(
            descricao,
            @"#INTEGRACAO\s*.*?\s*#FIMINTEGRACAO",
            "",
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        ).Trim();
    }

    private decimal ObterDecimal(Dictionary<string, string> dados, string chave)
    {
        if (!dados.TryGetValue(chave, out var valor))
            return 0m;

        return decimal.TryParse(
            valor.Replace(",", "."),
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out var resultado)
            ? resultado
            : 0m;
    }

    private string StatusToString(string status)
    {
      if (status.ToUpper() == "CONFIRMADA")
        return "1";
      else if (status.ToUpper() == "PENDENTE")
        return "2";
      else
        return "3";
    }

  }
}

