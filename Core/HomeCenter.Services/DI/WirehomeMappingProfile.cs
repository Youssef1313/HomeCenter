﻿using AutoMapper;
using HomeCenter.Model.Commands;
using HomeCenter.Model.Components;
using HomeCenter.Model.Events;
using HomeCenter.Conditions;
using HomeCenter.Core.ComponentModel.Areas;
using HomeCenter.Core.ComponentModel.Configuration;
using HomeCenter.Core.Utils;
using System.Collections.Generic;
using System.Linq;

namespace HomeCenter.Core.Services.DependencyInjection
{
    public class HomeCenterMappingProfile : Profile
    {
        public HomeCenterMappingProfile()
        {
            ShouldMapProperty = propInfo => (propInfo.CanWrite && propInfo.GetGetMethod(true).IsPublic) || propInfo.IsDefined(typeof(MapAttribute), false);

            CreateMap<ComponentDTO, Component>().ConstructUsingServiceLocator();
            CreateMap<AdapterReferenceDTO, AdapterReference>();
            CreateMap<TriggerDTO, Trigger>();
            CreateMap<CommandDTO, Command>();
            CreateMap<EventDTO, Event>();
            CreateMap<AreaDTO, Area>();
            CreateMap<ScheduleDTO, Schedule>();
            CreateMap<ConditionContainerDTO, ConditionContainer>().ForMember(s => s.Conditions, d => d.ResolveUsing<ConditionResolver>()).ConstructUsingServiceLocator();
        }
    }

    public class ConditionResolver : IValueResolver<ConditionContainerDTO, ConditionContainer, IList<IValidable>>
    {
        public IList<IValidable> Resolve(ConditionContainerDTO source, ConditionContainer destination, IList<IValidable> destMember, ResolutionContext context)
        {
            IList<IValidable> list = new List<IValidable>();
            var types = AssemblyHelper.GetAllTypes<IValidable>();

            foreach (var condition in source.Conditions)
            {
                var conditionType = types.FirstOrDefault(c => c.Name.IndexOf(condition.Type) > -1);

                var cons = conditionType.GetConstructors().FirstOrDefault();
                var args = new List<object>();

                foreach (var parameter in cons.GetParameters())
                {
                    args.Add(context.ConfigurationProvider.ServiceCtor(parameter.ParameterType));
                }

                if (conditionType != null)
                {
                    list.Add((IValidable)cons.Invoke(args.ToArray()));
                }
            }

            return list;
        }
    }
}