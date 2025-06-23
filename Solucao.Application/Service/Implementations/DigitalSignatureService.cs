using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Contracts.Response;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Service.Interfaces;

namespace Solucao.Application.Service.Implementations
{
	public class DigitalSignatureService : IDigitalSignatureService
	{
        private CalendarRepository calendarRepository;
        private DigitalSignatureEventsRepository eventosRepository;

        private DigitalSignatureRepository assinaturaRepository;
        private const string enviarDocumentosParaAssinar = "api/v2/processo/enviar-documento-para-assinar";

        public DigitalSignatureService(CalendarRepository _calendarRepository, DigitalSignatureRepository _assinaturaRepository, DigitalSignatureEventsRepository _eventosRepository)
		{
            calendarRepository = _calendarRepository;
            assinaturaRepository = _assinaturaRepository;
            eventosRepository = _eventosRepository;
        }

        public async Task<ValidationResult> EnviarDocumentoParaAssinar(Guid calendarId)
        {
            string apiRest = Environment.GetEnvironmentVariable("ApiArqSign");
            string appKey = Environment.GetEnvironmentVariable("AppKey");
            string subscriptionKey = Environment.GetEnvironmentVariable("SubscriptionKey");

            var locacao = await calendarRepository.GetById(calendarId);

            var assinatura = await assinaturaRepository.GetByCalendarId(calendarId);

            var mensagemPadrao = new DigitalSignatureMensagemPadrao
            {
                Titulo = "Contrato de Locação",
                Texto = $"Você está recebendo o seu contrato de locação para assinatura - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}"
            };

            var destinatarios = new List<DigitalSignatureDestinatario>();
            int contador = 0;

            foreach (var dest in locacao.Client.ClientDigitalSignatures)
            {
                var destinatario = new DigitalSignatureDestinatario
                {
                    IdTipoAcao = 1,
                    OrdemAssinatura = contador,
                    Nome = dest.Name,
                    Email = dest.Email,
                    AssinarOnline = new DigitalSignatureAssinarOnline
                    {
                        AssinarComo = 1,
                        PapelPessoaFisica = new List<string> { dest.PartyName },
                        IdTipoAssinatura = 1,
                        AssinaturaEletronica = new DigitalSignatureAssinaturaEletronica
                        {
                            ObrigarSignatarioInformarNome = true,
                            ObrigarSignatarioInformarNumeroDocumento = true,
                            TipoDocumentoAInformar = 1
                        }
                    }
                };
                destinatarios.Add(destinatario);
                contador++;

            }

            var documentos = new List<DigitalSignatureDocumento>();

            byte[] fileBytes = File.ReadAllBytes(locacao.ContractPath.Replace(".docx",".pdf"));
            string base64String = Convert.ToBase64String(fileBytes);

            DigitalSignatureDocumento documento = new DigitalSignatureDocumento
            {
                OrdemDocumento = 1,
                NomeComExtensao = locacao.FileNamePdf,
                Arquivo = base64String
            };
            documentos.Add(documento);

            DigitalSignatureRequest envioAssinatura = new DigitalSignatureRequest
            {
                NomeProcesso = assinatura.NomeProcesso,
                IdPasta = assinatura.IdPasta,
                IdResponsavel = assinatura.IdResponsavel,
                MensagemPadrao = mensagemPadrao,
                UsarOrdemAssinatura = false,
                UsarPosicaoAssinaturaAutomatica = true,
                Destinatarios = destinatarios,
                Documentos = documentos
            };

            string json = JsonSerializer.Serialize(envioAssinatura);

            string url = $"{apiRest}{enviarDocumentosParaAssinar}";

            using var client = new HttpClient();

            // Adicionando os headers necessários
            client.DefaultRequestHeaders.Add("AppKey", appKey);
            client.DefaultRequestHeaders.Add("SubscriptionKey", subscriptionKey);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Houve erro na assinatura do documento");

            string result = await response.Content.ReadAsStringAsync();

            var resposta = JsonSerializer.Deserialize<DigitalSignatureResponse>(result);

            assinatura.IdProcesso = resposta.idProcesso;

            await assinaturaRepository.Update(assinatura);

            return ValidationResult.Success;
        }

        public async Task<ValidationResult> EventosWebhook(DigitalSignatureResponse response)
        {
            var processo = await assinaturaRepository.GetById(response.idProcesso);
            DigitalSignatureEvents evento = new DigitalSignatureEvents();

            switch (response.idEvento)
            {
                case 1:
                    evento.DataHoraAtual = response.dataHoraAtual;
                    evento.Evento = response.evento;
                    evento.IdConta = response.idConta;
                    evento.IdProcesso = response.idProcesso;
                    evento.IdWebhook = response.idWebhook;
                    await eventosRepository.Add(evento);
                    processo.Status = "in_progress";
                    await assinaturaRepository.Update(processo);
                    return ValidationResult.Success;
                default:
                    break;
            }

            

            return null;
        }
    }
}

