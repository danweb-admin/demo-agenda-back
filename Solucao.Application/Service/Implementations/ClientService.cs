using AutoMapper;
using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Contracts.Response;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Interfaces;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Exceptions.Calendar;
using Solucao.Application.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Solucao.Application.Service.Implementations
{
    public class ClientService : IClientService
    {
        private IClientRepository clientRepository;
        private ClientEquipmentRepository clientEquipmentRepository;
        private IEquipamentRepository equipamentRepository;
        private EquipmentRelationshipRepository equipmentRelationshipRepository;
        private TimeValuesRepository timeValuesRepository;
        private ClientSpeficationRepository clientSpecificationRepository;
        private ClientDigialSignatureRepository assinaturaRepository;

        private readonly IMapper mapper;
        private List<string> timeList;

        public ClientService(IClientRepository _clientRepository, IMapper _mapper, IEquipamentRepository _equipamentRepository, EquipmentRelationshipRepository _equipmentRelationshipRepository, TimeValuesRepository _timeValuesRepository, ClientEquipmentRepository _clientEquipmentRepository, ClientSpeficationRepository _clientSpecificationRepository, ClientDigialSignatureRepository _assinaturaRepository)
        {
            clientRepository = _clientRepository;
            equipamentRepository = _equipamentRepository;
            equipmentRelationshipRepository = _equipmentRelationshipRepository;
            timeValuesRepository = _timeValuesRepository;
            clientEquipmentRepository = _clientEquipmentRepository;
            clientSpecificationRepository = _clientSpecificationRepository;
            assinaturaRepository = _assinaturaRepository;
            mapper = _mapper;

        }
        public Task<ValidationResult> Add(ClientViewModel client)
        {
            client.Id = Guid.NewGuid();
            client.CreatedAt = DateTime.Now;
            var _client = mapper.Map<Client>(client);

            return clientRepository.Add(_client);
        }

        public async Task<IEnumerable<ClientViewModel>> GetAll(bool ativo, string search)
        {
            var result = mapper.Map<IEnumerable<ClientViewModel>>(await clientRepository.GetAll(ativo, search));

            foreach (var item in result)
            {
                foreach (var _item in item.ClientEquipment)
                {
                    _item.TimeValues = _item.TimeValues.OrderBy(x => x.Time_).ToList();
                }
            }

            return result;
        }

        public async Task<ClientViewModel> GetById(Guid? Id)
        {
            if (Id.HasValue)
            {
                var result = mapper.Map<ClientViewModel>(await clientRepository.GetById(Id.Value));

                foreach (var _item in result.ClientEquipment)
                {
                    _item.TimeValues = _item.TimeValues.OrderBy(x => x.Time_).ToList();
                }

                return result;
            }

            return new ClientViewModel
            {
                ClientEquipment = await ReturnClientEquip()
            };
            
        }

        public async Task<ValidationResult> Update(ClientViewModel client)
        {
            client.UpdatedAt = DateTime.Now;
            var _client = mapper.Map<Client>(client);

            await clientSpecificationRepository.RemoveAllByClient(_client.Id);

            return await clientRepository.Update(_client);
        }

        public async Task<decimal> GetValueByEquipament(Guid clientId, Guid equipamentId, string startTime, string endTime)
        {

            var client = await clientRepository.GetById(clientId);
            var equipament = await equipamentRepository.GetById(equipamentId);

            var rentalTime = Utils.Helpers.RentalTime(startTime, endTime);

            var rentalTimeString = Utils.Helpers.FormatTime((decimal)rentalTime);

            var result = await clientRepository.GetEquipmentValueByClient(client.Id, equipament.Id, rentalTimeString);

            if (result != 0)
                return result;

            throw new CalendarNoValueException("Não foi encontrado o valor para a Locação no cadastro do cliente");
        }

        public async Task AdjustEquipmentValues()
        {
            var clients = await clientRepository.GetAll(true, "");
            var equipmentRelationship = await equipmentRelationshipRepository.GetAll(true);
            createTimeList();

            foreach (var item in equipmentRelationship)
            {
                foreach (var client in clients)
                {
                    if (client.ClientEquipment.Any(x => x.EquipmentRelationshipId == item.Id))
                        continue;

                    var clientEquipment = new ClientEquipment
                    {
                        Id = Guid.NewGuid(),
                        ClientId = client.Id,
                        EquipmentRelationshipId = item.Id
                    };

                    await clientEquipmentRepository.Add(clientEquipment);
                    var timeValues = new List<TimeValue>();

                    foreach (var time in timeList)
                    {
                        var timeValue = new TimeValue
                        {
                            Id = Guid.NewGuid(),
                            Value = 0,
                            Time = time,
                            ClientEquipmentId = clientEquipment.Id

                        };

                        timeValues.Add(timeValue);
                    }

                    await timeValuesRepository.Add(timeValues);
                    
                }

            }
        }

        public async Task<IEnumerable<ClientEquipmentNamesViewModel>> ClientEquipment(string clientName)
        {
            var clients = await clientRepository.GetAll(true, clientName);
            var list = new List<ClientEquipmentNamesViewModel>();

            foreach (var client in clients)
            {
                var equips = await clientEquipmentRepository.GetAllByClient(client.Id);
                var equipNames = string.Join(',', equips.Select(x => x.EquipmentRelationship.Name));
                list.Add(new ClientEquipmentNamesViewModel {
                        ClientId = client.Id,
                        ClientName = client.Name,
                        EquipNames = equipNames
                });
            }
            return list;
        }

        public async Task<ValidationResult> ClientEquipmentSave(ClientEquipmentNamesViewModel viewModel)
        {
            var result = await clientEquipmentRepository.RemoveAllByClient(viewModel.ClientId);
            createTimeList();

            foreach (var item in viewModel.EquipmentId)
            {
                var equipmentRelationship = await equipmentRelationshipRepository.GetByName(item);

                var clientEquipment = new ClientEquipment
                {
                    Id = Guid.NewGuid(),
                    ClientId = viewModel.ClientId,
                    EquipmentRelationshipId = equipmentRelationship.Id
                };

                await clientEquipmentRepository.Add(clientEquipment);
                var timeValues = new List<TimeValue>();

                foreach (var time in timeList)
                {
                    var timeValue = new TimeValue
                    {
                        Id = Guid.NewGuid(),
                        Value = 0,
                        Time = time,
                        ClientEquipmentId = clientEquipment.Id

                    };

                    timeValues.Add(timeValue);
                }

                await timeValuesRepository.Add(timeValues);
            }

            return ValidationResult.Success;
        }

        public async Task<List<ClientEquipmentViewModel>> ReturnClientEquip()
        {
            var equipmentRelationship = mapper.Map<List<EquipmentRelationshipViewModel>>(await equipmentRelationshipRepository.GetAll(true));
            var clientEquip = new List<ClientEquipmentViewModel>();
            createTimeList();

            foreach (var item in equipmentRelationship)
            {
                
                var clientEquipment = new ClientEquipmentViewModel
                {
                    Id = Guid.NewGuid(),
                    EquipmentRelationshipId = item.Id.Value,
                    EquipmentRelationship = item,
                    Name = item.Name
                };
                    
                var timeValues = new List<TimeValueViewModel>();

                foreach (var time in timeList)
                {
                    var timeValue = new TimeValueViewModel
                    {
                        Id = Guid.NewGuid(),
                        Value = 0,
                        Time = time,
                        ClientEquipmentId = clientEquipment.Id

                    };

                    timeValues.Add(timeValue);
                }
                clientEquipment.TimeValues = timeValues;
                clientEquip.Add(clientEquipment);

            }
            return clientEquip;
        }

        public async Task MigrateClientValues()
        {
            var clients = await clientRepository.GetAll(true, "");

            foreach (var client in clients)
            {
                var split = client.EquipamentValues.Split("->");

                foreach (var line in split)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var strings = line.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    var equipamento = strings[0].Trim();

                    for (int i = 0; i < strings.Length; i++)
                    {
                        if (i == 0)
                            continue;

                        var hoursValues = strings[i].Replace("-", "–").Split("–");

                        if (hoursValues.Length < 2)
                            continue;

                        var hours = hoursValues[0].Trim();
                        var value = decimal.Parse(hoursValues[1].Trim().Replace(".", "").Replace(",", "."));

                        var hr = decimal.Parse(Regex.Replace(hours.Trim().Replace(",","."), @"[a-zA-Z]", ""));

                        var time = Utils.Helpers.FormatTime(hr);

                        var timeValue =  await timeValuesRepository.GetTimeValue(time, equipamento, client.Id);

                        if (timeValue != null)
                        {
                            timeValue.Value = value;
                            await timeValuesRepository.Update(timeValue);
                        }
                    }
                    //}
                }
            }

        }

        public async Task AddClientDigitalSignatures()
        {
            var clients = await clientRepository.GetAll(true, "");

            foreach (var cli in clients)
            {
                var destinatario1 = new ClientDigitalSignature
                {
                    Id = Guid.NewGuid(),
                    Name = "William Nascimento",
                    Email = "william.nascimento2010@gmail.com",
                    PartyName = "contratante",
                    ClientId = cli.Id,
                    CreatedAt = DateTime.Now,
                    Active = true,
                };
                await assinaturaRepository.Add(destinatario1);
                var destinatario2 = new ClientDigitalSignature
                {
                    Id = Guid.NewGuid(),
                    Name = "Daniele Nascimento",
                    Email = "danweb.softwares@gmail.com",
                    PartyName = "contratada",
                    ClientId = cli.Id,
                    CreatedAt = DateTime.Now,
                    Active = true,
                    
                };
                await assinaturaRepository.Add(destinatario2);
                
            }
        }

        private void createTimeList()
        {
            timeList = new List<string>
            {
                "00:30",
                "01:00",
                "01:30",
                "02:00",
                "02:30",
                "03:00",
                "03:30",
                "04:00",
                "04:30",
                "05:00",
                "05:30",
                "06:00",
                "06:30",
                "07:00",
                "07:30",
                "08:00",
                "08:30",
                "09:00",
                "09:30",
                "10:00",
                "10:30",
                "11:00",
                "11:30",
                "12:00",
                "12:30",
                "13:00",
                "13:30",
                "14:00"
            };

        }

        
    }
}
