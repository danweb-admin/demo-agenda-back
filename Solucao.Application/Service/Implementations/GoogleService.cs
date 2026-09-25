using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    private CityRepository cityRepository;
    private IClientRepository clientRepository;
    private IEquipamentRepository equipmentRepository;
    private UserRepository userRepository;

    public GoogleService(CalendarRepository _calendarRepository, IClientRepository _clientRepository, IEquipamentRepository _equipmentRepository, UserRepository _userRepository, ICalendarService _calendarService, CityRepository _cityRepository)
    {
      calendarRepository = _calendarRepository;
      clientRepository = _clientRepository;
      equipmentRepository = _equipmentRepository;
      userRepository = _userRepository;
      calendarService = _calendarService;
      cityRepository = _cityRepository;

    }

    public async Task ExtrairInformacoe(GoogleRequest request)
    {

      
        var locacao  = await calendarRepository.GetByGoogleEventId(request.Id);

        if (locacao == null)
          await InsereLocacao(request);
      
    }

    private async Task<bool> InsereLocacao(GoogleRequest request)
    {
        // Aparelho
        var aparelho = await equipmentRepository.GetByName(request.Aparelho.Trim());

        if (aparelho == null)
            throw new IntegrationException(
                $"Aparelho: {request.Aparelho.Trim()}, Aparelho não encontrado."
            );

        if (request.Titulo.Contains("CANCELADO"))
          throw new IntegrationException(
                $"Locacao: {request.Titulo}, Locacao cancelada."
            );

        // Remove CANCELADO
        var titulo = Regex.Replace(
            request.Titulo,
            @"\*+CANCELADO\*+",
            "",
            RegexOptions.IgnoreCase
        ).Trim();

        // ==========================================
        // OBTÉM OU CRIA O CLIENTE
        // ==========================================
        var cliente = await ObterOuCriarCliente(
            titulo,
            request.Descricao
        );

        if (cliente == null)
            throw new IntegrationException(
                $"Não foi possível identificar o locatário: {request.Titulo.Trim()}."
            );

        var user = await userRepository.GetByEmail("admin@admin.com");

        var horaInicio = DateTime.Parse(request.Inicio);
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

        var result = await calendarService.Add(locacao, user.Id);

        if (result == null)
            return true;

        return false;
    }

    private async Task<Client> ObterOuCriarCliente(string titulo,string descricao)
    {
        // ==========================================
        // 1. TENTA PELO TÍTULO COMPLETO
        // ==========================================

        var cliente = await clientRepository.GetByIntegrationName(titulo);

        if (cliente != null)
            return cliente;


        // ==========================================
        // 2. EXTRAI NOME E CIDADE DO TÍTULO
        // ==========================================

        var dadosTitulo = ExtrairDadosTitulo(titulo);

        var nome = dadosTitulo.Nome;
        var cidade = dadosTitulo.Cidade;


        // ==========================================
        // 3. TENTA PELO NOME
        // ==========================================

        if (!string.IsNullOrWhiteSpace(nome))
        {
            var list = await clientRepository.GetByIntegrationNameList(nome);

            if (list.Count() == 1)
              return list.FirstOrDefault();

            var cliente1 = list.FirstOrDefault(x => x.Specialty.ToUpper().Contains(cidade.ToUpper()));

            if (cliente1 != null)
                return cliente1;
        }


        // ==========================================
        // 4. EXTRAI DADOS DA DESCRIÇÃO
        // ==========================================

        var dadosDescricao = ExtrairDadosDescricao(descricao);


        // ==========================================
        // 5. SE O TÍTULO NÃO TIVER CIDADE,
        //    USA A CIDADE DA DESCRIÇÃO
        // ==========================================

        if (string.IsNullOrWhiteSpace(cidade))
            cidade = dadosDescricao.Cidade;

        // ==========================================
        // 4. LOCALIZA CIDADE
        // ==========================================

        var city = await cityRepository.GetCityByName(cidade.ToUpper());


        // ==========================================
        // 6. CRIA O CLIENTE
        // ==========================================

        cliente = new Client
        {
            Name = nome,
            Phone = dadosDescricao.Telefone,
            Address = dadosDescricao.Endereco,
            Complement = dadosDescricao.Complemento,
            Neighborhood = dadosDescricao.Bairro,
            City = city,
            State = city.State,
            ZipCode = dadosDescricao.Cep
        };


        // ==========================================
        // 7. SALVA
        // ==========================================

        clientRepository.Add(cliente);


        return cliente;
    }

    private DadosTituloCliente ExtrairDadosTitulo(string titulo)
    {
        var resultado = new DadosTituloCliente();

        if (string.IsNullOrWhiteSpace(titulo))
            return resultado;

        titulo = titulo.Trim();

        var separados = titulo.Split('-');

        if (separados.Length == 2)
        {
          resultado.Nome = separados[0];
          resultado.Cidade = separados[1];

          return resultado;
        }

        // Exemplo:
        // MPT-IVANA NOGUEIRA - PIRASSUNUNGA

        var match = Regex.Match(
            titulo,
            @"^[^-]+-\s*(.*?)\s*-\s*(.+)$",
            RegexOptions.IgnoreCase
        );

        if (match.Success)
        {
            resultado.Nome = match.Groups[1].Value.Trim();
            resultado.Cidade = match.Groups[2].Value.Trim();

            return resultado;
        }

        // Caso venha somente:
        // MPT-IVANA NOGUEIRA

        match = Regex.Match(
            titulo,
            @"^[^-]+-\s*(.+)$",
            RegexOptions.IgnoreCase
        );

        if (match.Success)
        {
            resultado.Nome = match.Groups[1].Value.Trim();
        }
        else
        {
            resultado.Nome = titulo.Trim();
        }

        return resultado;
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

    private DadosDescricaoCliente ExtrairDadosDescricao(string descricao)
    {
        var resultado = new DadosDescricaoCliente();

        if (string.IsNullOrWhiteSpace(descricao))
            return resultado;

        // Normaliza espaços, mas mantém o texto original para alguns casos
        var texto = Regex.Replace(descricao, @"\r\n|\r|\n", " ");
        texto = Regex.Replace(texto, @"\s+", " ").Trim();


        // ==========================================
        // TELEFONE
        // ==========================================

        var telefoneMatch = Regex.Match(
            texto,
            @"(?:TEL\.?|TELEFONE)\s*[:\-]?\s*(\(?\d{2}\)?\s*\d{4,5}[-\s]?\d{4})",
            RegexOptions.IgnoreCase
        );

        if (telefoneMatch.Success)
        {
            resultado.Telefone = NormalizarTelefone(
                telefoneMatch.Groups[1].Value
            );
        }


        // ==========================================
        // CEP
        // ==========================================

        var cepMatch = Regex.Match(
            texto,
            @"CEP\s*[:\-]?\s*(\d{5}-?\d{3})",
            RegexOptions.IgnoreCase
        );

        if (cepMatch.Success)
        {
            resultado.Cep = cepMatch.Groups[1].Value.Trim();
        }


        // ==========================================
        // ENDEREÇO
        // ==========================================

        var enderecoMatch = Regex.Match(
            texto,
            @"(?:ATENÇÃO\s+A\s+NOVO\s+ENDEREÇO|NOVO\s+ENDEREÇO)\s*:\s*(.*?)(?:CEP\s*[:\-]?\s*\d{5}-?\d{3}|$)",
            RegexOptions.IgnoreCase
        );

        if (enderecoMatch.Success)
        {
            var enderecoTexto = enderecoMatch.Groups[1].Value.Trim();

            enderecoTexto = Regex.Replace(
                enderecoTexto,
                @"\s+",
                " "
            ).Trim();

            resultado.Endereco = enderecoTexto;

            ExtrairPartesEndereco(
                enderecoTexto,
                resultado
            );
        }

        return resultado;
    }

    private string NormalizarTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return null;

        return Regex.Replace(telefone, @"\D", "");
    }

    private void ExtrairPartesEndereco( string endereco, DadosDescricaoCliente resultado)
    {
        if (string.IsNullOrWhiteSpace(endereco))
            return;

        // ==========================================
        // CIDADE E ESTADO
        // ==========================================

        var cidadeEstadoMatch = Regex.Match(
            endereco,
            @"(?:de\s+)?([^,]+),\s*(São Paulo|SP|Paraná|PR|Rio de Janeiro|RJ|Minas Gerais|MG)$",
            RegexOptions.IgnoreCase
        );

        if (cidadeEstadoMatch.Success)
        {
            resultado.Cidade =
                cidadeEstadoMatch.Groups[1].Value.Trim();

            resultado.Estado =
                ConverterEstadoParaSigla(
                    cidadeEstadoMatch.Groups[2].Value.Trim()
                );

            endereco = endereco
                .Substring(0, cidadeEstadoMatch.Index)
                .Trim();
        }


        // ==========================================
        // NÚMERO
        // ==========================================

        var numeroMatch = Regex.Match(
            endereco,
            @",\s*(\d+)",
            RegexOptions.IgnoreCase
        );


        // ==========================================
        // COMPLEMENTO / BAIRRO
        // ==========================================

        var complementoMatch = Regex.Match(
            endereco,
            @"(?:,\s*)?((?:sala|apto|apartamento|casa|bloco|conjunto)\s*[^,]*?)\s+(centro|[^,]+)$",
            RegexOptions.IgnoreCase
        );

        if (complementoMatch.Success)
        {
            resultado.Complemento =
                complementoMatch.Groups[1].Value.Trim();

            resultado.Bairro =
                complementoMatch.Groups[2].Value.Trim();

            endereco = endereco
                .Substring(0, complementoMatch.Index)
                .Trim();
        }
        else
        {
            // Tenta identificar "centro" no final
            var bairroMatch = Regex.Match(
                endereco,
                @"\b(centro)\b$",
                RegexOptions.IgnoreCase
            );

            if (bairroMatch.Success)
            {
                resultado.Bairro =
                    bairroMatch.Value.Trim();

                endereco = endereco
                    .Substring(0, bairroMatch.Index)
                    .Trim();
            }
        }


        // ==========================================
        // LIMPA ENDEREÇO
        // ==========================================

        resultado.Endereco = endereco.Trim();
    }

    private string ConverterEstadoParaSigla(string estado)
    {
        if (string.IsNullOrWhiteSpace(estado))
            return null;

        estado = estado.Trim().ToUpper();

        return estado switch
        {
            "SÃO PAULO" => "SP",
            "PARANÁ" => "PR",
            "RIO DE JANEIRO" => "RJ",
            "MINAS GERAIS" => "MG",
            "SANTA CATARINA" => "SC",
            "RIO GRANDE DO SUL" => "RS",
            "BAHIA" => "BA",
            "PERNAMBUCO" => "PE",
            "CEARÁ" => "CE",
            "GOIÁS" => "GO",
            "ESPÍRITO SANTO" => "ES",
            "MATO GROSSO" => "MT",
            "MATO GROSSO DO SUL" => "MS",
            "PARÁ" => "PA",
            "AMAZONAS" => "AM",
            "MARANHÃO" => "MA",
            "PARAÍBA" => "PB",
            "RIO GRANDE DO NORTE" => "RN",
            "ALAGOAS" => "AL",
            "SERGIPE" => "SE",
            "PIAUÍ" => "PI",
            "TOCANTINS" => "TO",
            "RONDÔNIA" => "RO",
            "ACRE" => "AC",
            "AMAPÁ" => "AP",
            "RORAIMA" => "RR",
            "DISTRITO FEDERAL" => "DF",
            _ => estado
        };
    }
  }
  public class DadosDescricaoCliente
  {
      public string Telefone { get; set; }

      public string Cep { get; set; }

      public string Endereco { get; set; }

      public string Complemento { get; set; }

      public string Bairro { get; set; }

      public string Cidade { get; set; }

      public string Estado { get; set; }
  }

  public class DadosTituloCliente
  {
      public string Nome { get; set; }
      public string Cidade { get; set; }
  }
  
}



