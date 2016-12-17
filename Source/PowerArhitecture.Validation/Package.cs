using System;
using System.Linq;
using System.Reflection;
using FluentValidation;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Validation.Internal;
using PowerArhitecture.Validation.Specifications;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Packaging;

namespace PowerArhitecture.Validation
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            var registration = Lifestyle.Singleton.CreateRegistration<ValidatorFactory>(container);
            container.AddRegistration(typeof(IValidatorFactory), registration);

            foreach (var assembly in Assembly.GetExecutingAssembly().GetDependentAssemblies().Where(o => !o.IsDynamic))
            {
                AssemblyScanner.FindValidatorsInAssembly(assembly)
                    .Where(match => !match.ValidatorType.IsInterface && !match.ValidatorType.IsAbstract)
                    .ForEach(match =>
                    {
                        var serviceType = match.ValidatorType.GetGenericType(typeof(IValidator<>));
                        if (serviceType == null)
                        {
                            return;
                        }
                        registration = Lifestyle.Singleton.CreateRegistration(match.ValidatorType, container);
                        container.AddRegistration(match.ValidatorType, registration);
                        container.AddRegistration(serviceType, registration);
                    });
            }
            container.RegisterConditional(typeof(IValidator<>), typeof(Validator<>), Lifestyle.Singleton, o => !o.Handled);
            container.RegisterDecorator(typeof(IValidator<>), typeof(ValidatorDecorator<>), Lifestyle.Singleton);

            // Convention for business rules
            Assembly.GetExecutingAssembly()
                .GetDependentAssemblies()
                .SelectMany(o => o.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IBusinessRule).IsAssignableFrom(t))
                .Select(t => new { Implementation = t, Services = t.GetInterfaces().Where(o => o.IsAssignableToGenericType(typeof(IBusinessRule<,>))) })
                .ForEach(o =>
                {
                    if (o.Implementation.IsGenericType)
                    {
                        var args = o.Implementation.GetGenericArguments();
                        switch (args.Length)
                        {
                            case 1:
                                container.AppendToCollection(typeof(IBusinessRule<,>).MakeGenericType(args[0], args[0]), o.Implementation);
                                return;
                            default:
                                throw new NotSupportedException(
                                    $"Business rules with more that one generic argument are not supported. Invalid business rule: {o.Implementation}" +
                                    "Hint: make the rule as an abstract type or modify the type to contain only one generic argument");
                        }
                    }
                    registration = Lifestyle.Transient.CreateRegistration(o.Implementation, container);
                    container.AddRegistration(o.Implementation, registration);
                    foreach (var serviceType in o.Services)
                    {
                        container.AppendToCollection(serviceType, registration);
                    }
                });

            ValidatorOptions.ValidatorSelectors.RulesetValidatorSelectorFactory = strings => new CustomRulesetValidatorSelector(strings);
        }
    }
}
