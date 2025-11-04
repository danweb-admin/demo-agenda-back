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
using DocumentFormat.OpenXml.Packaging;
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
        private CultureInfo cultureInfo = new CultureInfo("pt-BR");


        public GenerateContractService(IMapper _mapper, CalendarRepository _calendarRepository, ModelRepository _modelRepository, IClientRepository _clientRepository, ModelAttributesRepository _modelAttributesRepository, DigitalSignatureRepository _assinaturaRepository)
        {
            mapper = _mapper;
            calendarRepository = _calendarRepository;
            modelRepository = _modelRepository;
            clientRepository = _clientRepository;
            modelAttributesRepository = _modelAttributesRepository;
            assinaturaRepository = _assinaturaRepository;
        }

        public async Task<IEnumerable<CalendarViewModel>> GetAllByDayAndContractMade(DateTime date)
        {
            return mapper.Map<IEnumerable<CalendarViewModel>>(await calendarRepository.GetAllByDayAndConfirmed(date));
        }

        public async Task<byte[]> DownloadContract(Guid calendarId)
        {

            var calendar = await calendarRepository.GetById(calendarId);

            if (!File.Exists(calendar.ContractPath))
                throw new ContractNotFoundException("Contrato não encontrado.");

            return await File.ReadAllBytesAsync(calendar.ContractPath);

        }

        public async Task<ValidationResult> GenerateContract(GenerateContractRequest request)
        {
            var modelPath = Environment.GetEnvironmentVariable("ModelDocsPath");
            var contractPath = Environment.GetEnvironmentVariable("DocsPath");
            var useList = Environment.GetEnvironmentVariable("UseList");

            var t = await calendarRepository.GetById(request.CalendarId);

            var calendar = mapper.Map<CalendarViewModel>(await calendarRepository.GetById(request.CalendarId));
            calendar.RentalTime = CalculateMinutes(calendar.StartTime.Value, calendar.EndTime.Value);
            await SearchCustomerValue(calendar);
            calendar.TotalValue = calendar.Value + calendar.Freight - calendar.Discount + calendar.Additional1;

            var model = await modelRepository.GetByEquipament(calendar.EquipamentId);

            if (model == null)
                throw new ModelNotFoundException("Modelo de contrato para esse equipamento não encontrado.");

            var attributes = await modelAttributesRepository.GetAll();

            var contractFileName = FormatNameFile(calendar.Client.Name, calendar.Equipament.Name, calendar.Date);

            var copiedFile = await CopyFileStream(modelPath, contractPath, model.ModelFileName, contractFileName, calendar.Date);

            var result = ExecuteReplace(copiedFile, attributes, calendar);

            if (result)
            {
                calendar.ContractPath = copiedFile;
                calendar.UpdatedAt = DateTime.Now;
                calendar.FileNameDocx = contractFileName;
                calendar.FileNamePdf = contractFileName.Replace("docx","pdf");
                calendar.ContractMade = true;

                ConvertDocxToPdf(copiedFile, contractPath, calendar.Date);

                await calendarRepository.Update(mapper.Map<Calendar>(calendar));

                if (useList == "S")
                    await AddDigitalSignature(calendar.Id.Value, contractFileName);

                return ValidationResult.Success;
            }

            return new ValidationResult("Erro para gerar o contrato");
        }

        private string FormatNameFile(string locatarioName, string equipamentName, DateTime date)
        {
            var _locatarioName = locatarioName.Replace(" ", "");
            var _equipamentName = equipamentName.Replace(" ", "");
            var _date = date.ToString("dd-MM-yyyy");

            return $"{_locatarioName}-{_equipamentName}-{_date}.docx";
        }

        private async Task<string> CopyFileStream(string modelDirectory, string contractDirectory, string modelFileName, string fileName, DateTime date)
        {
            try
            {
                FileInfo inputFile = new FileInfo(modelDirectory + modelFileName);

                var yearMonth = date.ToString("yyyy-MM");
                var day = date.ToString("dd");

                var createdDirectory = $"{contractDirectory}/{yearMonth}/{day}";

                using (FileStream originalFileStream = inputFile.OpenRead())
                {
                    if (!Directory.Exists(createdDirectory))
                        Directory.CreateDirectory(createdDirectory);

                    var outputFileName = Path.Combine(createdDirectory, fileName);
                    using (FileStream outputFileStream = new FileStream(outputFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        await originalFileStream.CopyToAsync(outputFileStream);
                    }
                    return outputFileName;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw new Exception(ex.Message.ToString());
            }
            
        }

        private bool ExecuteReplace(string copiedFile, IEnumerable<ModelAttributes> attributes, CalendarViewModel calendar)
        {
            try
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(copiedFile, true))
                {
                    string docText = null;
                    using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                        docText = sr.ReadToEnd();

                    foreach (var item in attributes)
                    {
                        Regex regexText = new Regex(item.FileAttribute.Trim());
                        var valueItem = GetPropertieValue(calendar, item.TechnicalAttribute, item.AttributeType);
                        docText = regexText.Replace(docText, valueItem);
                    }

                    using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                        sw.Write(docText);

                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }

        }

        private string GetPropertieValue(object obj, string propertieName, string attrType)
        {
            // Dividir o nome da propriedade para acessar propriedades aninhadas
            string[] properties = propertieName.Split('.');

            object value = obj;

            // Iterar sobre as propriedades
            foreach (var prop in properties)
            {
                // Obter tipo do objeto atual
                Type type = value.GetType();

                // Obter propriedade pelo nome
                var propInfo = type.GetProperty(prop);

                // Se a propriedade não existir, retornar null
                if (propInfo == null)
                {
                    return null;
                }

                // Obter valor da propriedade
                value = propInfo.GetValue(value);
            }

            // Converter valor para string (assumindo que a propriedade é do tipo string)
            return FormatValue(value.ToString(), attrType, propertieName);
        }

        private string FormatValue(string value, string attrType, string propertie)
        {
            switch (attrType)
            {
                case "datetime":
                    return DateTime.Parse(value).ToString("dd/MM/yyyy");
                case "datetime_extenso":
                    var monthDay = DateTime.Parse(value).ToString("M", cultureInfo);
                    var year = DateTime.Parse(value).ToString("yyyy", cultureInfo);

                    return $"{monthDay} de {year}";
                case "time":
                    return DateTime.Parse(value).ToString("HH:mm");
                case "decimal":
                    return decimal.Parse(value).ToString("N2", cultureInfo);
                case "decimal_extenso":
                    return decimalExtenso(value);
                case "time_extenso":
                    return timeExtenso(value);
                default:
                    if (propertie.ToUpper().Contains("CPF"))
                        return Regex.Replace(value, @"^(\d{3})(\d{3})(\d{3})(\d{2})$", "$1.$2.$3-$4");
                    if (propertie.ToUpper().Contains("CNPJ"))
                        return Regex.Replace(value, @"^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$", "$1.$2.$3/$4-$5");
                    if (propertie.ToUpper().Contains("ZIPCODE"))
                        return Regex.Replace(value, @"^(\d{5})(\d{3})$", "$1-$2");
                    if (propertie.ToUpper().Contains("CELL"))
                        return FormatCelular(value);
                    return value;
            }
        }

        private string FormatCelular(string input)
        {
            // sempre 11 dígitos: 2 DDD + 9 número
            if (input.Length != 11) return input;
            return $"({input.Substring(0, 2)}) {input.Substring(2, 5)}-{input.Substring(7, 4)}";
        }

        private string decimalExtenso(string value)
        {
            var decimalSplit = decimal.Parse(value).ToString("n2").Split('.');
            var part1 = long.Parse(decimalSplit[0].Replace(",", "")).ToWords(cultureInfo).ToTitleCase(TitleCase.First);
            var part2 = int.Parse(decimalSplit[1]).ToWords(cultureInfo);

            if (part2 == "zero")
                return $"{part1} reais";
            return $"{part1} reais e {part2} centavos";
        }

        private string timeExtenso(string value)
        {
            var minutesTotal = int.Parse(value);

            int hours = minutesTotal / 60;
            int minutes = minutesTotal % 60;

            string result = "";

            if (hours == 0)
            {
                if (minutes > 0)
                    result += $"{minutes} {(minutes == 1 ? "minuto" : "minutos")}";
                return result;
            }

            result = $"{hours} {(hours == 1 ? "hora" : "horas")}";

            if (minutes > 0)
                result += $" e {minutes} {(minutes == 1 ? "minuto" : "minutos")}";

            return result;
        }

        private async Task SearchCustomerValue(CalendarViewModel calendar)
        {
            var useList = Environment.GetEnvironmentVariable("UseList");

            if (useList == "S")
                return;

            TimeSpan difference = calendar.EndTime.Value - calendar.StartTime.Value;
            var rentalTime = difference.TotalHours;

            var rentalTimeString = Utils.Helpers.FormatTime((decimal)rentalTime);

            var result = await clientRepository.GetEquipmentValueByClient(calendar.ClientId, calendar.EquipamentId, rentalTimeString);

            if (result != 0)
            {

                var specs = await clientRepository.GetSpecsByClient(calendar.ClientId);
                if (specs.Count() > 0)
                    calendar.Value = ValuesBySpecification(calendar, specs, result);
                else
                    calendar.Value = calendar.ValueWithoutSpec = result;
                return;
            }

            throw new CalendarNoValueException("Não foi encontrado o valor para a Locação no cadastro do cliente");

        }

        private decimal ValuesBySpecification(CalendarViewModel calendar, IEnumerable<ClientSpecification> specifications, decimal valueWithoutSpec)
        {
            TimeSpan difference = calendar.EndTime.Value - calendar.StartTime.Value;
            var rentalTime = difference.TotalHours;

            calendar.ValueWithoutSpec = valueWithoutSpec;
            var specs = calendar.CalendarSpecifications.Where(x => x.Active);

            foreach (var spec in specs)
            {
                foreach (var item in specifications)
                {
                    if (item.SpecificationId == spec.SpecificationId)
                    {
                        if (rentalTime <= item.Hours || item.Hours == 0)
                        {
                            calendar.Additional1 = item.Value;
                            return valueWithoutSpec;
                        }
                    }
                }
            }

            return valueWithoutSpec;
        }

        private int CalculateMinutes(DateTime startTime, DateTime endTime)
        {
            if (endTime < startTime)
                throw new ArgumentException("A data final deve ser maior ou igual à data inicial.");

            TimeSpan difference = endTime - startTime;
            return (int)difference.TotalMinutes;
        }

        private void ConvertDocxToPdf(string inputFilePath, string outputDirectory, DateTime date)
        {
            try
            {
                var yearMonth = date.ToString("yyyy-MM");
                var day = date.ToString("dd");

                var createdDirectory = $"{outputDirectory}/{yearMonth}/{day}";

                var process = new Process();
                process.StartInfo.FileName = "soffice";
                process.StartInfo.Arguments = $"--headless --convert-to pdf --outdir \"{createdDirectory}\" \"{inputFilePath}\"";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            
        }

        private async Task AddDigitalSignature(Guid calendarId, string inputFilePath)
        {
            var idPasta = Environment.GetEnvironmentVariable("IdPasta");
            var idResponsavel = Environment.GetEnvironmentVariable("IdResponsavel");

            Console.WriteLine($"idPasta: {idPasta}");
            Console.WriteLine($"idResponsavel: {idResponsavel}");

            var assinatura = new DigitalSignature();
            
            assinatura.Id = new Guid();
            assinatura.CalendarId = calendarId;
            assinatura.IdPasta = Guid.Parse("7dbdc3c0-ed1b-4f72-bb2a-b3520150071b");
            assinatura.IdResponsavel = Guid.Parse("49c4c9db-b3a9-4de1-b2d6-dc8ac9d99f03");
            assinatura.NomeProcesso = inputFilePath.Replace(".docx", ".pdf");
            assinatura.Status = "pending";
            assinatura.CreatedAt = DateTime.Now;

            

            await assinaturaRepository.Add(assinatura);
        }

    }
}

