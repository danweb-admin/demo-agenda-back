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
    public class EntityToViewModelMappingProfile : Profile
    {
        public EntityToViewModelMappingProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<Person, PersonViewModel>();
            CreateMap<Client, ClientViewModel>();
            CreateMap<Specification, SpecificationViewModel>();
            CreateMap<Equipament, EquipamentViewModel>();
            CreateMap<Calendar, CalendarViewModel>()
                .ForMember(dest => dest.ContractPath, opt => opt.ConvertUsing(new MarkDownConverter()));
                //.ForMember(dest => dest.Value, opt => opt.MapFrom(x => x.Value.ToString("n2").Replace(".",",")));
            CreateMap<StickyNote, StickyNoteViewModel>();
            CreateMap<Model, ModelViewModel>();
            CreateMap<ModelAttributes, ModelAttributeViewModel>();

            CreateMap<Consumable, ConsumableViewModel>();
            CreateMap<EquipamentConsumable, EquipamentConsumableViewModel>();
            CreateMap<CalendarEquipamentConsumable, CalendarEquipamentConsumablesViewModel>();
            CreateMap<CalendarSpecificationConsumables, CalendarSpecificationConsumablesViewModel>();

            CreateMap<EquipmentRelationship, EquipmentRelationshipViewModel>();

            CreateMap<ClientEquipment, ClientEquipmentViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.EquipmentRelationship.Name));
            
            CreateMap<TimeValue, TimeValueViewModel>()
                .ForMember(dest => dest.Time_, opt => opt.MapFrom(src => TimeSpan.Parse(src.Time)));

            CreateMap<ClientSpecification, ClientSpecificationViewModel>();





        }


    }

    public class MarkDownConverter : IValueConverter<string, string>
    {
        public string Convert(string sourceMember, ResolutionContext context)
        {
            
            var split = sourceMember?.Split('/');
            if (split == null || split.Length == 0)
                return string.Empty;
            var length = split.Length;

            return split[length - 1];
        }
    }
}
