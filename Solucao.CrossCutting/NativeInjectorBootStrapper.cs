using Microsoft.Extensions.DependencyInjection;
using Solucao.Application.Data;
using Solucao.Application.Data.Interfaces;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Service.Implementations;
using Solucao.Application.Service.Interfaces;
using System;
using System.Net.Http;

namespace Solucao.CrossCutting
{
    public static class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {

            // Services
            services.AddScoped<IMD5Service, MD5Service>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IStateService, StateService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<ISpecificationService, SpecificationService>();
            services.AddScoped<IEquipamentService, EquipamentService>();
            services.AddScoped<ICalendarService, CalendarService>();
            services.AddScoped<IStickyNoteService, StickyNoteService>();
            services.AddScoped<IModelService, ModelService>();
            services.AddScoped<IModelAttributesService, ModelAttributesService>();
            services.AddScoped<IModelConfigurationService, ModelConfigurationService>();
            services.AddScoped<IGenerateContractService, GenerateContractService>();
            services.AddScoped<IConsumableService, ConsumableService>();
            services.AddScoped<IEquipmentRelantionshipService, EquipmentRelationshipService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IDigitalSignatureService, DigitalSignatureService>();
            services.AddScoped<IPriceTableService, PriceTableService>();
            services.AddScoped<TokenService>();

            // Infra
            services.AddScoped<UserRepository>();
            services.AddScoped<IPersonRepository,PersonRepository>();
            services.AddScoped<IClientRepository,ClientRepository>();
            services.AddScoped<IStickyNoteRepository, StickyNoteRepository>();
            services.AddScoped<StateRepository>();
            services.AddScoped<CityRepository>();
            services.AddScoped<SpecificationRepository>();
            services.AddScoped<IEquipamentRepository,EquipamentRepository>();
            services.AddScoped<EquipamentSpecificationsRepository>();
            services.AddScoped<CalendarRepository>();
            services.AddScoped<ModelRepository>();
            services.AddScoped<ModelAttributesRepository>();
            services.AddScoped<AttributeTypesRepository>();
            services.AddScoped<TechnicalAttributesRepository>();
            services.AddScoped<HistoryRepository>();
            services.AddScoped<ConsumableRepository>();
            services.AddScoped<EquipmentRelationshipRepository>();
            services.AddScoped<TimeValuesRepository>();
            services.AddScoped<ClientEquipmentRepository>();
            services.AddScoped<ClientSpeficationRepository>();
            services.AddScoped<DashboardRepository>();
            services.AddScoped<DigitalSignatureRepository>();
            services.AddScoped<DigitalSignatureEventsRepository>();
            services.AddScoped<ClientDigialSignatureRepository>();
            services.AddScoped<PriceTableRepository>();

            services.AddScoped<SolucaoContext>();


        }
    }
}
