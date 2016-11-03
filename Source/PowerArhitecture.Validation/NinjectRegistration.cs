using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PowerArhitecture.Validation.Factories;
using PowerArhitecture.Validation.Specifications;
using FluentValidation;
using Ninject;
using Ninject.Modules;
using Ninject.Extensions.Conventions;
using Ninject.Infrastructure.Language;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Validation.Events;

namespace PowerArhitecture.Validation
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            Bind(typeof(IValidator<>), typeof(Validator<>), typeof(AbstractValidator<>))
                .To(typeof(Validator<>))
                .InSingletonScope()
                .OnActivation((ctx, validator) =>
                {
                    var type = validator.GetType();
                    var modelType = type.IsGenericType
                                ? type.GenericTypeArguments[0]
                                : null;
                    ctx.Kernel.Get<IEventPublisher>().Publish(new ValidatorCreatedEvent((IValidator)validator, modelType));
                });

            Bind<IValidatorFactory, IValidatorFactoryExtended, NinjectValidatorFactory>().To<NinjectValidatorFactory>().InSingletonScope();
            Bind<IValidatorEngine, ValidatorEngine>().To<ValidatorEngine>().InSingletonScope();

            foreach (var assembly in Assembly.GetExecutingAssembly().GetDependentAssemblies().Where(o => !o.IsDynamic))
            {
                AssemblyScanner.FindValidatorsInAssembly(assembly)
                    .Where(match => !match.ValidatorType.IsInterface)
                    .ForEach(match =>
                        Bind(match.ValidatorType.GetAllInterfaces()
                            .Union(match.ValidatorType.GetAllBaseTypes())
                            .Union(new[] {match.ValidatorType}).ToArray())
                            .To(match.ValidatorType)
                            .InSingletonScope()
                            .OnActivation((ctx, validator) =>
                            {
                                var modelType = match.InterfaceType.IsGenericType
                                    ? match.InterfaceType.GenericTypeArguments[0]
                                    : null;

                                if (modelType != null &&
                                    ctx.Kernel.GetBindings(typeof(IValidationContextFiller<>).MakeGenericType(modelType)).Any())
                                {
                                    ((dynamic) validator).HasValidationContextFiller = true;
                                }
                                ctx.Kernel.Get<IEventPublisher>().Publish(new ValidatorCreatedEvent((IValidator) validator, modelType));
                            })
                    );

            }

            // Convention for validation context filler
            Kernel.Bind(o => o
                .From(AppConfiguration.GetDomainAssemblies()
                    .Where(a => a.GetTypes().Any(t => t.IsAssignableToGenericType(typeof(IValidationContextFiller<>)))))
                .Select(t => !t.IsInterface && !t.IsAbstract && !t.IsGenericType && t.IsAssignableToGenericType(typeof(IValidationContextFiller<>)) &&
                    !Kernel.GetBindings(t).Any())
                .BindSelection((type, types) => new List<Type> {type}.Union(types))
                .Configure(syntax => syntax.InTransientScope()));

            ValidatorOptions.ValidatorSelectors.RulesetValidatorSelectorFactory = strings => new CustomRulesetValidatorSelector(strings); 
        }
    }
}
