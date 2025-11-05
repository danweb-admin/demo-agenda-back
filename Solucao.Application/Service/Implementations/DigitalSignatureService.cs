using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Newtonsoft.Json;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Contracts.Response;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Exceptions.DigitalSignature;
using Solucao.Application.Service.Interfaces;

namespace Solucao.Application.Service.Implementations
{
    public class DigitalSignatureService : IDigitalSignatureService
    {
        private CalendarRepository calendarRepository;
        private DigitalSignatureEventsRepository eventosRepository;

        private DigitalSignatureRepository assinaturaRepository;
        private readonly IMapper mapper;
        private const string prefixoFone = "+55";

        private const string enviarDocumentosParaAssinar = "api/v2/processo/enviar-documento-para-assinar";

        public DigitalSignatureService(CalendarRepository _calendarRepository, DigitalSignatureRepository _assinaturaRepository, DigitalSignatureEventsRepository _eventosRepository, IMapper _mapper)
        {
            calendarRepository = _calendarRepository;
            assinaturaRepository = _assinaturaRepository;
            eventosRepository = _eventosRepository;
            mapper = _mapper;
        }

        public async Task<ValidationResult> EnviarDocumentoParaAssinar(Guid calendarId)
        {
            string apiRest = Environment.GetEnvironmentVariable("ApiArqSign");
            string appKey = Environment.GetEnvironmentVariable("AppKey");
            string subscriptionKey = Environment.GetEnvironmentVariable("SubscriptionKey");
            string locadorEmail = Environment.GetEnvironmentVariable("LocadorEmail");
            string locadorTelefone = Environment.GetEnvironmentVariable("LocadorPhone");


            if (string.IsNullOrEmpty(appKey))
                throw new DigitalSignatureException("Você não tem permissão para enviar Assinatura Digital, contate o suporte.");

            var locacao = await calendarRepository.GetById(calendarId);

            var assinatura = await assinaturaRepository.GetByCalendarId(calendarId);

            if (assinatura.Status == "in_progress")
                throw new DigitalSignatureException("Contrato já enviado para Assinatura");

            var mensagemPadrao = new DigitalSignatureMensagemPadrao
            {
                Titulo = "Contrato de Locação",
                Texto = $"Você está recebendo o seu contrato de locação para assinatura - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}"
            };

            var destinatarios = new List<DigitalSignatureDestinatario>();
            int contador = 1;

            //if (locacao.Client.ClientDigitalSignatures.Count() == 0)
            //    throw new DigitalSignatureException("Signatário não configurado, verifique cadastro do cliente");

            AddLocador(ref destinatarios);
                
            var destinatario = new DigitalSignatureDestinatario();

            destinatario.IdTipoAcao = 1;
            destinatario.OrdemAssinatura = contador;
            
            destinatario.Nome = locacao.Client.Name;

            if (string.IsNullOrEmpty(locadorEmail))
                destinatario.Telefone = prefixoFone + locacao.Client.CellPhone;
            if (string.IsNullOrEmpty(locadorTelefone))
                destinatario.Email = locacao.Client.Email;

            destinatario.AlterarNotificacoes = 1;
            destinatario.RetornarLinkProcesso = 1;

            var assinarOnline = new DigitalSignatureAssinarOnline();
            assinarOnline.AssinarComo = 1;
            //if (dest.IsPF)
                assinarOnline.PapelPessoaFisica = new List<string> { "Locatário" };
            //else
            //    assinarOnline.PapelPessoaJuridica = new List<string> { dest.PartyName };
            assinarOnline.IdTipoAssinatura = 1;
            var assinaturaEletronica = new DigitalSignatureAssinaturaEletronica
            {
                ObrigarSignatarioInformarNome = true,
                ObrigarSignatarioInformarNumeroDocumento = true,
                TipoDocumentoAInformar = 1
            };
            assinarOnline.AssinaturaEletronica = assinaturaEletronica;
            destinatario.AssinarOnline = assinarOnline;
            destinatarios.Add(destinatario);
            contador++;

            var documentos = new List<DigitalSignatureDocumento>();

            byte[] fileBytes = File.ReadAllBytes(locacao.ContractPath.Replace(".docx", ".pdf"));
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

            string json = System.Text.Json.JsonSerializer.Serialize(envioAssinatura);

            string url = $"{apiRest}{enviarDocumentosParaAssinar}";

            using var client = new HttpClient();

            // Adicionando os headers necessários
            client.DefaultRequestHeaders.Add("AppKey", appKey);
            client.DefaultRequestHeaders.Add("SubscriptionKey", subscriptionKey);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("response: " + await response.Content.ReadAsStringAsync());
                throw new DigitalSignatureException("Houve erro na assinatura do documento");
            }
                
            string result = await response.Content.ReadAsStringAsync();

            var resposta = JsonConvert.DeserializeObject<DigitalSignatureResponse>(result);

            if (resposta.Destinatarios.Any())
                await AdicionaLinkProcesso(resposta.IdProcesso, resposta.Destinatarios);

            assinatura.IdProcesso = resposta.IdProcesso;
            assinatura.Status = "in_progress";



            await assinaturaRepository.Update(assinatura);

            return ValidationResult.Success;
        }

        public async Task<ValidationResult> EventosWebhook(string response)
        {

            var webhookResponse = JsonConvert.DeserializeObject<DigitalSignatureResponse>(response);
            var processo = await assinaturaRepository.GetById(webhookResponse.IdProcesso);
            DigitalSignatureEvents evento = new DigitalSignatureEvents();

            var idEvento = webhookResponse.IdEvento;
            var isEventoComum = new[] { 1, 2, 5, 6, 7, 8 }.Contains(idEvento);
            var isEventoComSignatario = new[] { 3, 4 }.Contains(idEvento);

            if (isEventoComum || isEventoComSignatario)
            {
                await PreencherEventoAsync(evento, webhookResponse, incluirSignatario: isEventoComSignatario);
                await eventosRepository.Add(evento);

                processo.Status = ProcessStatus(idEvento);
                await assinaturaRepository.Update(processo);

                return ValidationResult.Success;
            }

            throw new DigitalSignatureException("Houve erro no webhook");
        }

        public async Task<IEnumerable<DigitalSignatureEventsViewModel>> HistoricoAssinatura(Guid CalendarId)
        {
            return mapper.Map<IEnumerable<DigitalSignatureEventsViewModel>>(await assinaturaRepository.GetHistoryByCalendarId(CalendarId));
           
        }

        private async Task AdicionaLinkProcesso(Guid IdProcesso, ICollection<DigitalSignatureDestinatarioResponse> Destinatarios)
        {
            foreach (var item in Destinatarios)
            {
                var evento = new DigitalSignatureEvents
                {
                    DataHoraAtual = DateTime.Now,
                    Evento = $"Nome: {item.NomeSignatario}, LinkProcesso: {item.LinkProcesso}",
                    IdProcesso = IdProcesso
                };

                await eventosRepository.Add(evento);
            }
        }

        private void AddLocador(ref List<DigitalSignatureDestinatario> destinatarios)
        {
            string locadorName = Environment.GetEnvironmentVariable("LocadorName");
            string locadorEmail = Environment.GetEnvironmentVariable("LocadorEmail");
            string locadorTelefone = Environment.GetEnvironmentVariable("LocadorPhone");


            if (string.IsNullOrEmpty(locadorName))
                throw new DigitalSignatureException("Nome do Locador não configurado.");

            //if (string.IsNullOrEmpty(locadorTelefone))
            //    throw new DigitalSignatureException("Telefone do Locador não configurado.");

            var destinatario = new DigitalSignatureDestinatario();

            destinatario.IdTipoAcao = 1;
            destinatario.OrdemAssinatura = 0;
            destinatario.Nome = locadorName;
            if (string.IsNullOrEmpty(locadorEmail))
                destinatario.Telefone = prefixoFone + locadorTelefone;
            if (string.IsNullOrEmpty(locadorTelefone))
                destinatario.Email = locadorEmail;
            destinatario.AlterarNotificacoes = 1;
            destinatario.RetornarLinkProcesso = 1;


            var assinarOnline = new DigitalSignatureAssinarOnline();
            assinarOnline.AssinarComo = 1;
            assinarOnline.PapelPessoaJuridica = new List<string> { "Locador"};
            assinarOnline.IdTipoAssinatura = 1;
            var assinaturaEletronica = new DigitalSignatureAssinaturaEletronica
            {
                ObrigarSignatarioInformarNome = true,
                ObrigarSignatarioInformarNumeroDocumento = true,
                TipoDocumentoAInformar = 1
            };
            assinarOnline.AssinaturaEletronica = assinaturaEletronica;
            destinatario.AssinarOnline = assinarOnline;
            destinatarios.Add(destinatario);


        }

        private async Task PreencherEventoAsync(DigitalSignatureEvents evento, DigitalSignatureResponse webhook, bool incluirSignatario = false)
        { 

            evento.DataHoraAtual = DateTime.Now;
            evento.Evento = await DescribeEvent(webhook.IdEvento);

            if (incluirSignatario && webhook.Signatarios?.Any() == true)
            {
                var signatario = webhook.Signatarios.First();
                evento.Evento += $" - Nome: {signatario.Nome} - Email: {signatario.Email}";
            }

            
            evento.IdProcesso = webhook.IdProcesso;
        }

        private string ProcessStatus(int idEvento)
        {
            return idEvento switch
            {
                1 or 3 => "in_progress",
                2 => "failed",
                4 => "refused",
                5 => "cancelled",
                6 => "expired",
                7 => "resent",
                8 => "completed",
                _ => "no_status"
            };
        }

        private Task<string> DescribeEvent(int idEvento)
        {
            var description = idEvento switch
            {
                1 => "Processo Enviado",
                2 => "Processo com falha de envio",
                3 => "Processo assinado por algum signatário",
                4 => "Processo recusado por algum signatário",
                5 => "Processo cancelado pelo remetente",
                6 => "Processo expirado",
                7 => "Processo reenviado",
                8 => "Processo assinado/concluído por todos os signatários",
                _ => "Descrição do evento não encontrada"
            };

            return Task.FromResult(description);
        }

        
    }

}
