using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PowerArhitecture.Validation.Factories;
using PowerArhitecture.Validation.Specifications;
using FluentValidation;
using Ninject.Modules;
using Ninject.Extensions.Conventions;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.Validation
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            Bind(typeof(IValidator<>)).To(typeof(Validator<>));

            Bind<IValidatorFactory, IValidatorFactoryExtended, NinjectValidatorFactory>().To<NinjectValidatorFactory>().InSingletonScope();
            Bind<IValidatorEngine, ValidatorEngine>().To<ValidatorEngine>().InSingletonScope();

            foreach (var assembly in Assembly.GetExecutingAssembly().GetDependentAssemblies())
            {
                AssemblyScanner.FindValidatorsInAssembly(assembly)
                    .Where(match => !match.ValidatorType.IsInterface)
                .ForEach(match => Bind(match.ValidatorType.GetAllInterfaces().Union(new[] { match.ValidatorType }).ToArray())
                    .To(match.ValidatorType)
                    .InSingletonScope()
                    .OnActivation((ctx, validator) =>
                    {
                        if (match.InterfaceType.IsGenericType && 
                            ctx.Kernel.GetBindings(typeof(IValidationContextFiller<>).MakeGenericType(
                                match.InterfaceType.GenericTypeArguments[0])).Any())
                        {
                            ((dynamic) validator).HasValidationContextFiller = true;
                        }
                    }));
            }

            //Convenction for validation context filler
            Kernel.Bind(o => o
                .From(AppConfiguration.GetDomainAssemblies()
                    .Where(a => a.GetTypes().Any(t => t.IsAssignableToGenericType(typeof(IValidationContextFiller<>)))))
                .Select(t => !t.IsInterface && !t.IsAbstract && !t.IsGenericType && t.IsAssignableToGenericType(typeof(IValidationContextFiller<>)) &&
                    !Kernel.GetBindings(t).Any())
                .BindSelection((type, types) => new List<Type> { type }.Union(types))
                .Configure(syntax => syntax.InTransientScope()));

            ValidatorOptions.ValidatorSelectors.RulesetValidatorSelectorFactory = strings => new PARulesetValidatorSelector(strings); 
        }
    }
}
