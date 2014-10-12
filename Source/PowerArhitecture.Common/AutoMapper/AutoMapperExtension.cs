using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BAF.Common.Events;
using Bootstrap.Extensions;
using Bootstrap.Extensions.Containers;
using Ninject;
using Ninject.Syntax;
using Bootstrapper = Bootstrap.Bootstrapper;

namespace BAF.Common.AutoMapper
{
    public class AutoMapperExtension : IBootstrapperExtension
    {
        private readonly IRegistrationHelper _registrationHelper;

        public AutoMapperExtension(IRegistrationHelper registrationHelper)
        {
            Bootstrapper.Excluding.Assembly("AutoMapper");
            _registrationHelper = registrationHelper;
        }

        public void Run()
        {
            List<Profile> profiles;
            var eventAggregator = Bootstrapper.ContainerExtension.Resolve<IEventAggregator>();
            var kernel = Bootstrapper.ContainerExtension.Resolve<IKernel>();

            eventAggregator.SendMessage(new AutoMapperBeforeInitializingEvent());

            if (!kernel.GetBindings(typeof (Profile)).Any())
            {
                profiles = new List<Profile>();
            }
            else if (Bootstrapper.ContainerExtension != null && Bootstrapper.Container != null)
            {
                profiles = Bootstrapper.ContainerExtension.ResolveAll<Profile>().ToList();
            }
            else
            {
                profiles = _registrationHelper.GetInstancesOfTypesImplementing<Profile>();
            }
            Mapper.Initialize(c =>
            {
                eventAggregator.SendMessage(new AutoMapperInitializingEvent(c));
                c.ConstructServicesUsing(t => kernel.Get(t));
                ((IConfigurationProvider)c).TypeMapCreated += (sender, args) => 
                    eventAggregator.SendMessage(new AutoMapperTypeMapCreatedEvent(args));
                profiles.ForEach(c.AddProfile);
            });
        }

        public void Reset()
        {
            Mapper.Reset();
        }
    }
}
