using System.Linq;
using System.Reflection;
using PowerArhitecture.Validation.Factories;
using PowerArhitecture.Validation.Specifications;
using Castle.DynamicProxy.Internal;
using FluentValidation;
using Ninject.Modules;

namespace PowerArhitecture.Validation
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            Bind(typeof(IValidator<>)).To(typeof(PAValidator<>));

            Bind<IValidatorFactory, IValidatorFactoryExtended>().To<NinjectValidatorFactory>();
            Bind<IValidatorEngine>().To<PAValidatorEngine>(); //No singleton because of ChildKernel

            foreach (var assembly in Assembly.GetExecutingAssembly().GetDependentAssemblies())
            {
                AssemblyScanner.FindValidatorsInAssembly(assembly)
                .ForEach(match => Bind(match.ValidatorType.GetAllInterfaces().Union(new[] { match.ValidatorType }).ToArray())
                    .To(match.ValidatorType));
            }
        }
    }
}
