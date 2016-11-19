using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Validation.Events;
using PowerArhitecture.Validation.Factories;
using PowerArhitecture.Validation.Specifications;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace PowerArhitecture.Validation
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterInitializer<IValidator>(validator =>
            {
                var type = validator.GetType();
                var genType = type.GetInterfaces()
                    .FirstOrDefault(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IValidator<>));
                Type modelType = null;
                if (genType != null)
                {
                    modelType = genType.GenericTypeArguments[0];
                    if (type.IsAssignableToGenericType(typeof(Validator<>)))
                    {
                        var fillerType = typeof(IValidationContextFiller<>).MakeGenericType(modelType);
                        if (container.GetCurrentRegistrations().Any(o => o.ServiceType == fillerType))
                        {
                            ((dynamic)validator).HasValidationContextFiller = true;
                        }
                    }
                }
                container.GetInstance<IEventPublisher>().Publish(new ValidatorCreatedEvent(validator, modelType));
            });

            var registration = Lifestyle.Singleton.CreateRegistration<ValidatorFactory>(container);
            container.AddRegistration(typeof(IValidatorFactory), registration);

            foreach (var assembly in Assembly.GetExecutingAssembly().GetDependentAssemblies().Where(o => !o.IsDynamic))
            {
                AssemblyScanner.FindValidatorsInAssembly(assembly)
                    .Where(match => !match.ValidatorType.IsInterface)
                    .ForEach(match =>
                    {
                        registration = Lifestyle.Singleton.CreateRegistration(match.ValidatorType, container);
                        container.AddRegistration(match.ValidatorType, registration);
                        var serviceTypes = match.ValidatorType.GetInterfaces()
                            .Where(o => o.IsAssignableToGenericType(typeof(IValidator<>)))
                            .Union(match.ValidatorType.GetAllBaseTypes()
                                .Where(o => o.IsAssignableToGenericType(typeof(Validator<>))));
                        foreach (var serviceType in serviceTypes)
                        {
                            container.AddRegistration(serviceType, registration);
                        }
                    });
            }


            //registration = Lifestyle.Singleton.CreateRegistration(typeof(Validator<>), container);
            //container.RegisterConditional(typeof(IValidator<>), registration, o => !o.Handled);
            //container.RegisterConditional(typeof(Validator<>), registration, o => !o.Handled);
            container.RegisterConditional(typeof(IValidator<>), typeof(Validator<>), Lifestyle.Singleton, o => !o.Handled);

            // Convention for validation context filler
            AppConfiguration.GetDomainAssemblies()
                .SelectMany(o => o.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType && t.IsAssignableToGenericType(typeof(IValidationContextFiller<>)))
                .Select(t => new { Implementation = t, Services = t.GetInterfaces() })
                .ForEach(o =>
                {
                    registration = Lifestyle.Transient.CreateRegistration(o.Implementation, container);
                    container.AddRegistration(o.Implementation, registration);
                    foreach (var serviceType in o.Services)
                    {
                        container.AddRegistration(serviceType, registration);
                    }
                });

            ValidatorOptions.ValidatorSelectors.RulesetValidatorSelectorFactory = strings => new CustomRulesetValidatorSelector(strings);
        }
    }
}
