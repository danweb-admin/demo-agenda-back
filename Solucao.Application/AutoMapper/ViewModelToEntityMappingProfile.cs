using AutoMapper;
using Solucao.Application.Contracts;
using Solucao.Application.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.AutoMapper
{
    public class ViewModelToEntityMappingProfile : Profile
    {
        public ViewModelToEntityMappingProfile()
        {
            CreateMap<UserViewModel, User>().ForMember(x => x.Password, x => x.Ignore());
            CreateMap<PersonViewModel, Person>();
            CreateMap<ClientViewModel, Client>();
            CreateMap<SpecificationViewModel, Specification>();
            CreateMap<EquipamentViewModel, Equipament>();
            CreateMap<CalendarViewModel, Calendar>();
            CreateMap<StickyNoteViewModel, StickyNote>();
            CreateMap<ModelViewModel, Model>();
            CreateMap<ModelAttributeViewModel, ModelAttributes>();
            CreateMap<ConsumableViewModel, Consumable>();
            CreateMap<EquipamentConsumableViewModel, EquipamentConsumable>();
            CreateMap<CalendarEquipamentConsumablesViewModel, CalendarEquipamentConsumable>();
            CreateMap<CalendarSpecificationConsumablesViewModel, CalendarSpecificationConsumables>();
            CreateMap<EquipmentRelationshipViewModel,EquipmentRelationship > ();
            CreateMap<ClientEquipmentViewModel,ClientEquipment>();
            CreateMap<TimeValueViewModel,TimeValue>();
            CreateMap<ClientSpecificationViewModel,ClientSpecification>();
            CreateMap<ClientDigitalSignatureViewModel, ClientDigitalSignature>();

        }
    }
}
