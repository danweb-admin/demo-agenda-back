using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Humanizer;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Interfaces;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Exceptions.Calendar;
using Solucao.Application.Exceptions.Model;
using Solucao.Application.Service.Interfaces;
using Solucao.Application.Utils;
using Calendar = Solucao.Application.Data.Entities.Calendar;

namespace Solucao.Application.Service.Implementations
{
    public class GenerateContractService : IGenerateContractService
    {
        private readonly IMapper mapper;
        private readonly CalendarRepository calendarRepository;
        private readonly ModelRepository modelRepository;
        private readonly DigitalSignatureRepository assinaturaRepository;
        private readonly ModelAttributesRepository modelAttributesRepository;
        private readonly IClientRepository clientRepository;

        private readonly CultureInfo cultureInfo = new CultureInfo("pt-BR");

        public GenerateContractService(
            IMapper mapper,
            CalendarRepository calendarRepository,
            ModelRepository modelRepository,
            IClientRepository clientRepository,
            ModelAttributesRepository modelAttributesRepository,
            DigitalSignatureRepository assinaturaRepository)
        {
            this.mapper = mapper;
            this.calendarRepository = calendarRepository;
            this.modelRepository = modelRepository;
            this.clientRepository = clientRepository;
            this.modelAttributesRepository = modelAttributesRepository;
            this.assinaturaRepository = assinaturaRepository;
        }

        // =========================
        // PUBLIC
        // =========================

        public async Task<ValidationResult> GenerateContract(
            GenerateContractRequest request,
            IEnumerable<CalendarViewModel> listaLocacoes)
        {
            var modelPath = Environment.GetEnvironmentVariable("ModelDocsPath");
            var contractPath = Environment.GetEnvironmentVariable("DocsPath");
            var useList = Environment.GetEnvironmentVariable("UseList");

            var calendar = mapper.Map<CalendarViewModel>(
                await calendarRepository.GetById(request.CalendarId));

            calendar.Client.Document = string.IsNullOrEmpty(calendar.Client.Cnpj)
                ? FormatValue(calendar.Client.Cpf, "", "CPF")
                : FormatValue(calendar.Client.Cnpj, "", "CNPJ");

            calendar.RentalTime = CalculateMinutes(
                calendar.StartTime.Value,
                calendar.EndTime.Value);

            await SearchCustomerValue(calendar);

            calendar.TotalValue =
                calendar.Value + calendar.Freight - calendar.Discount + calendar.Additional1;

            listaLocacoes ??= new List<CalendarViewModel> { calendar };

            var locacoesOrdenadas = listaLocacoes
                .OrderBy(c => c.StartTime)
                .ToList();

            calendar.ListaLocacoes = string.Join("\n",
                locacoesOrdenadas.Select(c =>
                    $"{c.StartTime:dd/MM/yyyy} – das {c.StartTime:HH:mm} às {c.EndTime:HH:mm}")
            );

            var datasLocacao = locacoesOrdenadas.Select(x => x.Date.Date).ToList();

            var model = await modelRepository.GetByEquipament(calendar.EquipamentId)
                ?? throw new ModelNotFoundException("Modelo de contrato não encontrado.");

            var attributes = await modelAttributesRepository.GetAll();

            var contractFileName = FormatNameFile(
                calendar.Client.Name,
                calendar.Equipament.Name,
                calendar.Date);

            var copiedFile = await CopyFileStream(
                modelPath,
                contractPath,
                model.ModelFileName,
                contractFileName,
                calendar.Date);

            // =========================
            // 🔥 OPENXML – ABERTURA ÚNICA
            // =========================
            using (var doc = WordprocessingDocument.Open(copiedFile, true))
            {
                ExecuteReplace(doc, attributes, calendar);
                AddMultipleDatesBlock(doc, datasLocacao);
                ReplaceWithParagraphs(doc, calendar.ListaLocacoes);

                doc.MainDocumentPart.Document.Save();
            }

            // =========================
            // PDF
            // =========================
            ConvertDocxToPdf(copiedFile, contractPath, calendar.Date);

            calendar.ContractPath = copiedFile;
            calendar.FileNameDocx = contractFileName;
            calendar.FileNamePdf = contractFileName.Replace(".docx", ".pdf");
            calendar.ContractMade = true;
            calendar.UpdatedAt = DateTime.Now;
            calendar.Status = "1";

            await calendarRepository.UpdateContract(mapper.Map<Calendar>(calendar));

            if (useList == "S")
                await AddDigitalSignature(calendar.Id.Value, calendar.FileNamePdf);

            return ValidationResult.Success;
        }

        // =========================
        // OPENXML HELPERS
        // =========================

        private void ExecuteReplace(
            WordprocessingDocument doc,
            IEnumerable<ModelAttributes> attributes,
            CalendarViewModel calendar)
        {
            string docText;

            using (var sr = new StreamReader(doc.MainDocumentPart.GetStream()))
                docText = sr.ReadToEnd();

            foreach (var item in attributes)
            {
                var value = GetPropertieValue(calendar, item.TechnicalAttribute, item.AttributeType);
                if (string.IsNullOrEmpty(value)) continue;

                docText = Regex.Replace(docText, item.FileAttribute.Trim(), value);
            }

            using (var sw = new StreamWriter(doc.MainDocumentPart.GetStream(FileMode.Create)))
                sw.Write(docText);
        }

        private void ReplaceWithParagraphs(WordprocessingDocument doc, string text)
        {
            var body = doc.MainDocumentPart.Document.Body;

            var placeholder = body.Descendants<Paragraph>()
                .FirstOrDefault(p => p.InnerText.Contains("#ListaLocacoes"));

            if (placeholder == null) return;

            foreach (var line in text.Split('\n'))
            {
                body.InsertAfter(
                    new Paragraph(
                        new ParagraphProperties(
                            new SpacingBetweenLines
                            {
                                Line = "260",
                                Before = "20",
                                After = "20"
                            }),
                        new Run(new Text(line) { Space = SpaceProcessingModeValues.Preserve })
                    ),
                    placeholder
                );
            }

            placeholder.Remove();
        }

        private void AddMultipleDatesBlock(WordprocessingDocument doc, List<DateTime> dates)
        {
            var body = doc.MainDocumentPart.Document.Body;

            var placeholder = body.Descendants<Text>()
                .FirstOrDefault(t => t.Text.Contains("#BLOCO_LOCACOES_MULTIPLAS#"));

            if (placeholder == null || dates.Count <= 1)
            {
                placeholder?.Remove();
                return;
            }

            var paragraph = placeholder.Ancestors<Paragraph>().First();
            placeholder.Text = "";

            paragraph.InsertAfterSelf(CreateMultipleDatesTable(dates));
        }

        private Table CreateMultipleDatesTable(IEnumerable<DateTime> dates)
        {
            var table = new Table();

            table.AppendChild(new TableProperties(
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder { Val = BorderValues.Single, Size = 4 },
                    new RightBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                )
            ));

            // Cabeçalho
            table.Append(CreateRow("Data", header: true));

            foreach (var date in dates.OrderBy(d => d))
            {
                table.Append(CreateRow(date.ToString("dd/MM/yyyy")));
            }

            return table;
        }

        private TableRow CreateRow(string value, bool header = false)
        {
            return new TableRow(CreateCell(value, header));
        }

        private TableCell CreateCell(string text, bool bold = false)
        {
            var run = new Run(new Text(text));

            if (bold)
                run.RunProperties = new RunProperties(new Bold());

            return new TableCell(new Paragraph(run));
        }



        // =========================
        // FILE / PDF
        // =========================

        private async Task<string> CopyFileStream(
            string modelDirectory,
            string contractDirectory,
            string modelFileName,
            string fileName,
            DateTime date)
        {
            var inputPath = Path.Combine(modelDirectory, modelFileName);

            var yearMonth = date.ToString("yyyy-MM");
            var day = date.ToString("dd");
            var createdDirectory = Path.Combine(contractDirectory, yearMonth, day);

            Directory.CreateDirectory(createdDirectory);

            var outputPath = Path.Combine(createdDirectory, fileName);

            await using (var source = new FileStream(
                inputPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                await using (var destination = new FileStream(
                    outputPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None))
                {
                    await source.CopyToAsync(destination);
                    await destination.FlushAsync(); // 🔥 MUITO IMPORTANTE
                }
            }

            return outputPath;
        }


        private void ConvertDocxToPdf(string inputFile, string outputDir, DateTime date)
        {
            var soffice = Environment.GetEnvironmentVariable("PathSoffice");

            var targetDir = Path.Combine(
                outputDir,
                date.ToString("yyyy-MM"),
                date.ToString("dd"));

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = $"{soffice}soffice",
                    Arguments = $"--headless --convert-to pdf --outdir \"{targetDir}\" \"{inputFile}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();
        }

        // =========================
        // UTIL
        // =========================

        private string FormatNameFile(string client, string equip, DateTime date) =>
            $"{client.Replace(" ", "")}-{equip.Replace(" ", "")}-{date:dd-MM-yyyy}.docx";

        private int CalculateMinutes(DateTime start, DateTime end) =>
            (int)(end - start).TotalMinutes;

        private string GetPropertieValue(object obj, string prop, string type)
        {
            foreach (var p in prop.Split('.'))
            {
                var info = obj.GetType().GetProperty(p);
                if (info == null) return null;
                obj = info.GetValue(obj);
            }

            return obj == null ? "" : FormatValue(obj.ToString(), type, prop);
        }

        private string FormatValue(string value, string type, string prop)
        {
            return type switch
            {
                "datetime" => DateTime.Parse(value).ToString("dd/MM/yyyy"),
                "decimal" => decimal.Parse(value).ToString("N2", cultureInfo),
                _ => value
            };
        }

        private async Task SearchCustomerValue(CalendarViewModel calendar)
        {
            var useList = Environment.GetEnvironmentVariable("UseList");

            if (useList == "S")
                return;
                
            var result = await clientRepository.GetEquipmentValueByClient(
                calendar.ClientId,
                calendar.EquipamentId,
                Utils.Helpers.FormatTime((decimal)(calendar.EndTime - calendar.StartTime).Value.TotalHours));

            if (result == 0)
                throw new CalendarNoValueException("Valor não encontrado.");

            calendar.Value = result;
        }

        private async Task AddDigitalSignature(Guid calendarId, string pdfFile)
        {
            var pasta = Environment.GetEnvironmentVariable("IdPasta");
            var resp = Environment.GetEnvironmentVariable("IdResponsavel");

            if (string.IsNullOrEmpty(pasta) || string.IsNullOrEmpty(resp)) return;

            await assinaturaRepository.Add(new DigitalSignature
            {
                CalendarId = calendarId,
                IdPasta = Guid.Parse(pasta),
                IdResponsavel = Guid.Parse(resp),
                NomeProcesso = pdfFile,
                Status = "pending",
                CreatedAt = DateTime.Now
            });
        }

        public async Task<IEnumerable<CalendarViewModel>> GetAllByDayAndContractMade(DateTime date)
        {
            var calendars = await calendarRepository.GetAllByDayAndConfirmed(date);
            return mapper.Map<IEnumerable<CalendarViewModel>>(calendars);
        }


        public async Task<ValidationResult> GenerateMultipleContract(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return new ValidationResult("Nenhuma locação informada.");

            var idsArray = ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(Guid.Parse)
                .ToArray();

            var listaLocacoes = await calendarRepository.GetAllById(idsArray);

            if (!listaLocacoes.Any())
                return new ValidationResult("Nenhuma locação encontrada.");

            var listaViewModel = mapper.Map<IEnumerable<CalendarViewModel>>(listaLocacoes);

            // ⚠️ IMPORTANTE: processar SEQUENCIALMENTE
            foreach (var locacao in listaViewModel)
            {
                var request = new GenerateContractRequest
                {
                    CalendarId = locacao.Id.Value
                };

                var result = await GenerateContract(request, listaViewModel);

                if (result != ValidationResult.Success)
                    return result;
            }

            return ValidationResult.Success;
        }


        public async Task<IEnumerable<CalendarViewModel>> BuscarLocacoes(
        Guid cliendId,
        Guid equipmentId,
        DateTime startDate,
        DateTime endDate)
        {
            var calendars = await calendarRepository
                .GetAllByClient(cliendId, equipmentId, startDate, endDate);

            return mapper.Map<IEnumerable<CalendarViewModel>>(calendars);
        }


        public async Task<byte[]> DownloadContract(Guid calendarId)
        {
            var calendar = await calendarRepository.GetById(calendarId);

            if (calendar == null || string.IsNullOrEmpty(calendar.ContractPath))
                throw new ContractNotFoundException("Contrato não encontrado.");

            if (!File.Exists(calendar.ContractPath))
                throw new ContractNotFoundException("Arquivo do contrato não existe no servidor.");

            return await File.ReadAllBytesAsync(calendar.ContractPath);
        }

    }
}
