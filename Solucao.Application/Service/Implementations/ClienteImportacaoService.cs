using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Solucao.Application.Data;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Results;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class ClienteImportacaoService
{
    private readonly SolucaoContext _context;

    public ClienteImportacaoService(SolucaoContext context)
    {
        _context = context;
    }

    public async Task<ImportacaoClientesResult> ImportarAsync(IFormFile arquivo)
    {
        var resultado = new ImportacaoClientesResult();

        if (arquivo == null || arquivo.Length == 0)
        {
            resultado.Erros.Add("Arquivo não informado.");
            return resultado;
        }

        if (!Path.GetExtension(arquivo.FileName)
                .Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            resultado.Erros.Add("O arquivo precisa ser um Excel .xlsx.");
            return resultado;
        }

        // ============================================================
        // CARREGA ESTADOS E CIDADES
        // ============================================================

        var estados = await _context.States
            .AsNoTracking()
            .ToListAsync();

        var cidades = await _context.Cities
            .AsNoTracking()
            .ToListAsync();

        // ============================================================
        // CARREGA DOCUMENTOS JÁ CADASTRADOS
        // ============================================================

        var documentosExistentes = await _context.Clients
            .AsNoTracking()
            .Where(x => x.Cpf != null || x.Cnpj != null)
            .Select(x => new
            {
                x.Cpf,
                x.Cnpj
            })
            .ToListAsync();

        var cpfsExistentes = documentosExistentes
            .Where(x => !string.IsNullOrWhiteSpace(x.Cpf))
            .Select(x => SomenteNumeros(x.Cpf))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet();

        var cnpjsExistentes = documentosExistentes
            .Where(x => !string.IsNullOrWhiteSpace(x.Cnpj))
            .Select(x => SomenteNumeros(x.Cnpj))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet();

        // ============================================================
        // ABRE EXCEL COM OPENXML
        // ============================================================

        using var stream = arquivo.OpenReadStream();

        using var document = SpreadsheetDocument.Open(
            stream,
            false);

        var workbookPart = document.WorkbookPart;

        if (workbookPart == null)
        {
            resultado.Erros.Add(
                "Nenhuma planilha encontrada no arquivo.");

            return resultado;
        }

        // ============================================================
        // PEGA A PRIMEIRA PLANILHA
        // ============================================================

        var sheet = workbookPart.Workbook
            .Sheets?
            .Elements<Sheet>()
            .FirstOrDefault();

        if (sheet == null)
        {
            resultado.Erros.Add(
                "Nenhuma planilha encontrada no arquivo.");

            return resultado;
        }

        var worksheetPart = (WorksheetPart)workbookPart
            .GetPartById(sheet.Id!);

        var sheetData = worksheetPart.Worksheet
            .GetFirstChild<SheetData>();

        if (sheetData == null)
        {
            resultado.Erros.Add(
                "A planilha está vazia.");

            return resultado;
        }

        // ============================================================
        // PRIMEIRA LINHA
        // ============================================================

        var primeiraLinha = sheetData
            .Elements<Row>()
            .FirstOrDefault();

        if (primeiraLinha == null)
        {
            resultado.Erros.Add(
                "A planilha está vazia.");

            return resultado;
        }

        // ============================================================
        // MAPEIA COLUNAS
        // ============================================================

        var colunas = new Dictionary<string, int>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var cell in primeiraLinha.Elements<Cell>())
        {
            var nomeColuna = ObterValorCelula(
                cell,
                workbookPart).Trim();

            if (!string.IsNullOrWhiteSpace(nomeColuna))
            {
                var numeroColuna =
                    ObterNumeroColuna(cell.CellReference);

                colunas[nomeColuna] = numeroColuna;
            }
        }

        // ============================================================
        // VALIDA COLUNAS OBRIGATÓRIAS
        // ============================================================

        var colunasObrigatorias = new[]
        {
            "Cidade",
            "UF",
            "Endereço",
            "Bairro",
            "CEP"
        };

        foreach (var coluna in colunasObrigatorias)
        {
            if (!colunas.ContainsKey(coluna))
            {
                resultado.Erros.Add(
                    $"A coluna obrigatória '{coluna}' não foi encontrada.");
            }
        }

        if (resultado.Erros.Any())
            return resultado;

        // ============================================================
        // ÚLTIMA LINHA
        // ============================================================

        var ultimaLinha = sheetData
    .Elements<Row>()
    .Select(x => (int)(x.RowIndex?.Value ?? 0))
    .DefaultIfEmpty(1)
    .Max();

        var numeroPrimeiraLinha =
    (int)(primeiraLinha.RowIndex?.Value ?? 1);

        // ============================================================
        // PROCESSA AS LINHAS
        // ============================================================

        for (int numeroLinha = numeroPrimeiraLinha + 1;
             numeroLinha <= ultimaLinha;
             numeroLinha++)
        {
            resultado.TotalLinhas++;

            try
            {
                var row = sheetData
                    .Elements<Row>()
                    .FirstOrDefault(x =>
                        x.RowIndex?.Value == numeroLinha);

                if (row == null)
                {
                    resultado.Ignorados++;
                    continue;
                }

                var linha = LerLinha(
                    row,
                    colunas,
                    workbookPart);

                // ====================================================
                // IGNORA LINHA VAZIA
                // ====================================================

                if (string.IsNullOrWhiteSpace(linha.NomeFantasia) &&
                    string.IsNullOrWhiteSpace(linha.RazaoSocial) &&
                    string.IsNullOrWhiteSpace(linha.Documento))
                {
                    resultado.Ignorados++;
                    continue;
                }

                // ====================================================
                // DOCUMENTO
                // ====================================================

                var documento =
                    SomenteNumeros(linha.Documento);

                //if (string.IsNullOrWhiteSpace(documento))
                //{
                //    resultado.Ignorados++;

                //    resultado.Erros.Add(
                //        $"Linha {numeroLinha}: CPF/CNPJ não informado.");

                //    continue;
                //}

                // ====================================================
                // IDENTIFICA CPF OU CNPJ
                // ====================================================

                string cpf = null;
                string cnpj = null;

                if (documento.Length == 11)
                {
                    cpf = documento;

                    if (cpfsExistentes.Contains(cpf))
                    {
                        resultado.Ignorados++;

                        resultado.Erros.Add(
                            $"Linha {numeroLinha}: CPF {cpf} já cadastrado.");

                        continue;
                    }
                }
                else if (documento.Length == 14)
                {
                    cnpj = documento;

                    if (cnpjsExistentes.Contains(cnpj))
                    {
                        resultado.Ignorados++;

                        resultado.Erros.Add(
                            $"Linha {numeroLinha}: CNPJ {cnpj} já cadastrado.");

                        continue;
                    }
                }
                //else
                //{
                //    resultado.Ignorados++;

                //    resultado.Erros.Add(
                //        $"Linha {numeroLinha}: documento '{linha.Documento}' inválido.");

                //    continue;
                //}

                // ====================================================
                // ESTADO
                // ====================================================

                var uf = Normalizar(linha.Uf);

                var estado = estados.FirstOrDefault(x =>
                    Normalizar(x.Sigla) == uf);

                if (estado == null)
                {
                    resultado.Ignorados++;

                    resultado.Erros.Add(
                        $"Linha {numeroLinha}: estado '{linha.Uf}' não encontrado.");

                    continue;
                }

                // ====================================================
                // CIDADE
                // ====================================================

                var nomeCidade =
                    Normalizar(linha.Cidade);

                var cidade = cidades.FirstOrDefault(x =>
                    x.StateId == estado.Id &&
                    Normalizar(x.Nome) == nomeCidade);

                if (cidade == null)
                {
                    resultado.Ignorados++;

                    resultado.Erros.Add(
                        $"Linha {numeroLinha}: cidade '{linha.Cidade}' " +
                        $"não encontrada para o estado '{linha.Uf}'.");

                    continue;
                }

                // ====================================================
                // NOME
                // ====================================================

                var nome =
                    !string.IsNullOrWhiteSpace(linha.NomeFantasia)
                        ? linha.NomeFantasia.Trim()
                        : linha.RazaoSocial?.Trim();

                if (string.IsNullOrWhiteSpace(nome))
                {
                    resultado.Ignorados++;

                    resultado.Erros.Add(
                        $"Linha {numeroLinha}: nome do cliente não informado.");

                    continue;
                }

                // ====================================================
                // SPECIALTY
                // ====================================================

                var specialty = MontarSpecialty(
                    nome,
                    cidade.Nome);

                // ====================================================
                // ENDEREÇO
                // ====================================================

                var (endereco, numero) =
                    SepararEndereco(linha.Endereco);

                // ====================================================
                // TELEFONE
                // ====================================================

                var celular =
                    LimparTelefone(linha.Celular);

                var telefone =
                    LimparTelefone(linha.Telefone);

                if (string.IsNullOrWhiteSpace(celular))
                {
                    celular = telefone;
                }

                // ====================================================
                // OBSERVAÇÃO
                // ====================================================

                var observacao =
                    Normalizar(linha.Observacao);

                // ====================================================
                // CRIA CLIENTE
                // ====================================================

                var cliente = new Client
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Active = true,

                    Name = nome,

                    CellPhone = celular,
                    Phone = telefone,
                    Email = null,

                    Address = endereco,
                    Number = numero ?? "S/N",
                    Complement = null,

                    Neighborhood =
                        linha.Bairro?.Trim(),

                    CityId = cidade.Id,
                    StateId = estado.Id,

                    IsPhysicalPerson =
                        documento.Length == 11,

                    IsAnnualContract = null,
                    IsReceipt = null,

                    NameForReceipt = nome,

                    HasAirConditioning = null,

                    

                    TakeTransformer = null,

                    HasTechnique = null,
                    TechniqueOption1 = null,
                    TechniqueOption2 = null,

                    LandMark = null,
                    Responsible = null,

                    Specialty = specialty,

                    ClinicName = null,
                    ClinicCellPhone = null,

                    ZipCode =
                        linha.Cep?.Trim(),

                    Secretary = null,

                    Cpf = cpf,
                    Cnpj = cnpj,

                    Rg = null,
                    Ie = null,

                    EquipamentValues = null
                };

                _context.Clients.Add(cliente);

                // ====================================================
                // EVITA DUPLICIDADE NA PRÓPRIA PLANILHA
                // ====================================================

                try
                {
                    await _context.SaveChangesAsync();

                    if (cpf != null)
                        cpfsExistentes.Add(cpf);

                    if (cnpj != null)
                        cnpjsExistentes.Add(cnpj);

                    resultado.Importados++;
                }
                catch (DbUpdateException ex)
                {
                    resultado.Ignorados++;

                    var mensagem = ex.InnerException?.Message
                                   ?? ex.Message;

                    resultado.Erros.Add(
                        $"Linha {numeroLinha}: erro ao salvar cliente '{nome}': {mensagem}");

                    _context.Entry(cliente).State =
                        EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                resultado.Ignorados++;

                resultado.Erros.Add(
                    $"Linha {numeroLinha}: {ex.Message}");
            }
        }

        // ============================================================
        // SALVA NO BANCO
        // ============================================================

        await _context.SaveChangesAsync();

        return resultado;
    }

    // ============================================================
    // LÊ UMA LINHA DO EXCEL
    // ============================================================

    private ClienteImportacaoExcel LerLinha(
        Row row,
        Dictionary<string, int> colunas,
        WorkbookPart workbookPart)
    {
        return new ClienteImportacaoExcel
        {
            RazaoSocial = GetValor(
                row,
                colunas,
                "Razão Social",
                workbookPart),

            NomeFantasia = GetValor(
                row,
                colunas,
                "Nome Fantasia",
                workbookPart),

            Documento = GetValor(
                row,
                colunas,
                "CNPJ",
                workbookPart),

            Telefone = GetValor(
                row,
                colunas,
                "Telefone",
                workbookPart),

            Cidade = GetValor(
                row,
                colunas,
                "Cidade",
                workbookPart),

            Uf = GetValor(
                row,
                colunas,
                "UF",
                workbookPart),

            Observacao = GetValor(
                row,
                colunas,
                "Obs.",
                workbookPart),

            Endereco = GetValor(
                row,
                colunas,
                "Endereço",
                workbookPart),

            Bairro = GetValor(
                row,
                colunas,
                "Bairro",
                workbookPart),

            Cep = GetValor(
                row,
                colunas,
                "CEP",
                workbookPart),

            Celular = GetValor(
                row,
                colunas,
                "Celular/Whatss",
                workbookPart)
        };
    }

    // ============================================================
    // OBTÉM VALOR DA COLUNA
    // ============================================================

    private string GetValor(
        Row row,
        Dictionary<string, int> colunas,
        string coluna,
        WorkbookPart workbookPart)
    {
        if (!colunas.TryGetValue(
                coluna,
                out var numeroColuna))
        {
            return null;
        }

        var cell = row
            .Elements<Cell>()
            .FirstOrDefault(x =>
                ObterNumeroColuna(x.CellReference)
                == numeroColuna);

        if (cell == null)
            return null;

        return ObterValorCelula(
            cell,
            workbookPart)
            ?.Trim();
    }

    // ============================================================
    // OBTÉM VALOR REAL DA CÉLULA
    // ============================================================

    private string ObterValorCelula(
        Cell cell,
        WorkbookPart workbookPart)
    {
        if (cell == null)
            return string.Empty;

        var valor =
            cell.CellValue?.Text
            ?? cell.InnerText
            ?? string.Empty;

        // ========================================================
        // STRING COMPARTILHADA
        // ========================================================

        if (cell.DataType?.Value ==
            CellValues.SharedString)
        {
            var sharedStringPart =
                workbookPart.SharedStringTablePart;

            if (sharedStringPart == null)
                return valor;

            if (int.TryParse(
                    valor,
                    out var index))
            {
                var item =
                    sharedStringPart
                        .SharedStringTable?
                        .Elements<SharedStringItem>()
                        .ElementAtOrDefault(index);

                return item?.InnerText
                    ?? string.Empty;
            }
        }

        // ========================================================
        // BOOLEAN
        // ========================================================

        if (cell.DataType?.Value ==
            CellValues.Boolean)
        {
            return valor == "1"
                ? "TRUE"
                : "FALSE";
        }

        return valor;
    }

    // ============================================================
    // CONVERTE A REFERÊNCIA DA CÉLULA PARA NÚMERO DA COLUNA
    //
    // A1  = 1
    // B1  = 2
    // Z1  = 26
    // AA1 = 27
    // AB1 = 28
    // ============================================================

    private int ObterNumeroColuna(
        StringValue referencia)
    {
        if (referencia == null ||
            string.IsNullOrWhiteSpace(referencia.Value))
        {
            return 0;
        }

        var letras = new string(
            referencia.Value
                .TakeWhile(char.IsLetter)
                .ToArray());

        var numero = 0;

        foreach (var letra in letras)
        {
            numero =
                numero * 26 +
                (letra - 'A' + 1);
        }

        return numero;
    }

    // ============================================================
    // MONTA SPECIALTY
    // ============================================================

    private string MontarSpecialty(
        string nome,
        string cidade)
    {
        var nomes = nome
            .Trim()
            .Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

        var primeiroNome =
            nomes.First();

        var ultimoNome =
            nomes.Length > 1
                ? nomes.Last()
                : primeiroNome;

        return
            $"{primeiroNome} {ultimoNome} - {cidade}";
    }

    // ============================================================
    // NORMALIZA TEXTO
    // ============================================================

    private string Normalizar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return string.Empty;

        texto = texto
            .Trim()
            .ToUpperInvariant();

        var normalized = texto.Normalize(
            NormalizationForm.FormD);

        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var categoria =
                CharUnicodeInfo.GetUnicodeCategory(c);

            if (categoria != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        texto = sb.ToString();

        // Trata apóstrofos como espaço.
        texto = texto
            .Replace("'", " ")
            .Replace("’", " ");

        // Qualquer caractere que não seja
        // letra, número ou espaço vira espaço.
        texto = Regex.Replace(
            texto,
            @"[^A-Z0-9\s]",
            " ");

        // Remove espaços duplicados.
        texto = Regex.Replace(
            texto,
            @"\s+",
            " ");

        return texto.Trim();
    }

    // ============================================================
    // SOMENTE NÚMEROS
    // ============================================================

    private string SomenteNumeros(
        string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return null;

        return new string(
            valor
                .Where(char.IsDigit)
                .ToArray());
    }

    // ============================================================
    // TELEFONE
    // ============================================================

    private string LimparTelefone(
        string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return null;

        return SomenteNumeros(telefone);
    }

    // ============================================================
    // SEPARA ENDEREÇO E NÚMERO
    // ============================================================

    private (string Endereco, string Numero)
        SepararEndereco(
            string enderecoCompleto)
    {
        if (string.IsNullOrWhiteSpace(
                enderecoCompleto))
        {
            return (null, null);
        }

        enderecoCompleto =
            enderecoCompleto.Trim();

        // Remove hífen no final.
        enderecoCompleto =
            Regex.Replace(
                enderecoCompleto,
                @"\s*-\s*$",
                "");

        // Procura número depois de vírgula.
        var match =
            Regex.Match(
                enderecoCompleto,
                @",\s*(\d+)\s*$");

        if (match.Success)
        {
            var numero =
                match.Groups[1].Value;

            var endereco =
                enderecoCompleto
                    .Substring(
                        0,
                        match.Index)
                    .Trim()
                    .TrimEnd(',');

            return (
                endereco,
                numero);
        }

        // Caso não tenha vírgula.
        match =
            Regex.Match(
                enderecoCompleto,
                @"^(.*?)(?:\s+)(\d+)$");

        if (match.Success)
        {
            return (
                match.Groups[1]
                    .Value
                    .Trim(),

                match.Groups[2]
                    .Value
                    .Trim());
        }

        return (
            enderecoCompleto,
            null);
    }

    // ============================================================
    // INTERPRETA OBSERVAÇÕES
    // ============================================================

    private bool? Possui220V(
        string observacao)
    {
        if (string.IsNullOrWhiteSpace(
                observacao))
        {
            return null;
        }

        if (observacao.Contains("220V"))
            return true;

        if (observacao.Contains("110V"))
            return false;

        return null;
    }

    private bool? PossuiEscada(
        string observacao)
    {
        if (string.IsNullOrWhiteSpace(
                observacao))
        {
            return null;
        }

        if (observacao.Contains("ESCADA"))
            return true;

        return null;
    }
}

