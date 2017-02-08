using System;
using System.Linq;
using System.Reflection;
using FluentValidation;
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
            var validatorCache = new ValidatorCache();
            container.RegisterSingleton(validatorCache);

            var registration = Lifestyle.Singleton.CreateRegistration<ValidatorFactory>(container);
            container.AddRegistration(typeof(IValidatorFactory), registration);

            foreach (var assembly in Assembly.GetExecutingAssembly().GetDependentAssemblies())
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
                        if (args.Length > 2)
                        {
                            throw new NotSupportedException(
                                    $"Business rules with more that two generic argument are not supported. Invalid business rule: {o.Implementation}" +
                                    "Hint: make the rule as an abstract type or modify the type to contain two generic arguments or less");
                        }
                        // Register the implementation as we will need the registration when adding root/child producers
                        container.Register(o.Implementation, o.Implementation);
                        validatorCache.AddGenericBusinessRule(o.Implementation);
                        foreach (var serviceType in o.Services)
                        {
                            container.AppendToCollection(serviceType, o.Implementation);
                        }
                        return;
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
