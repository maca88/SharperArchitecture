using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.DataAccess.MappingSteps;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Steps;
using PowerArhitecture.DataAccess.Specifications;

namespace PowerArhitecture.DataAccess.Configurations
{
    public class AutoMappingConfiguration : DefaultAutomappingConfiguration, IFluentAutoMappingConfiguration
    {
        private readonly List<Assembly> _mappingStepsAssembiles;
        private Predicate<Type> _shouldMapTypePredicate;
        private Predicate<Member> _shouldMapMemberPredicate;

        public AutoMappingConfiguration()
        {
            _mappingStepsAssembiles = new List<Assembly>();
        }

        public IFluentAutoMappingConfiguration ShouldMapType(Predicate<Type> predicate)
        {
            _shouldMapTypePredicate = predicate;
            return this;
        }

        public IFluentAutoMappingConfiguration ShouldMapMember(Predicate<Member> predicate)
        {
            _shouldMapMemberPredicate = predicate;
            return this;
        }

        public IFluentAutoMappingConfiguration AddStepAssembly(Assembly assembly)
        {
            _mappingStepsAssembiles.Add(assembly);
            return this;
        }

        public IFluentAutoMappingConfiguration AddStepAssemblies(IEnumerable<Assembly> assemblies)
        {
            _mappingStepsAssembiles.AddRange(assemblies);
            return this;
        }

        public override bool ShouldMap(Type type)
        {
            if (type.GetCustomAttribute<IncludeAttribute>() != null)
            {
                return true;
            }
            return base.ShouldMap(type) && typeof(IEntity).IsAssignableFrom(type) && type.GetCustomAttribute<IgnoreAttribute>(false) == null &&
                (_shouldMapTypePredicate == null || _shouldMapTypePredicate(type));
        }

        public override bool AbstractClassIsLayerSupertype(Type type)
        {
            return type.GetCustomAttribute<IncludeAttribute>() == null;
        }

        public override bool ShouldMap(Member member)
        {
            if (member.IsProperty)
            {
                var propInfo = (PropertyInfo)member.MemberInfo;
                if (propInfo.GetCustomAttribute<IgnoreAttribute>() != null) 
                    return false;
                if (propInfo.GetCustomAttribute<IncludeAttribute>() != null)
                    return true;
                var getMethod = propInfo.GetGetMethod(true);
                if (getMethod.IsFamilyOrAssembly && !getMethod.IsFamily && !getMethod.IsAssembly) //ture for protected internal properties - .NET BUG?
                    return member.CanWrite;
            }

            return member.CanWrite && base.ShouldMap(member) && (_shouldMapMemberPredicate == null || _shouldMapMemberPredicate(member));
        }

        public override IEnumerable<IAutomappingStep> GetMappingSteps(AutoMapper mapper, FluentNHibernate.Conventions.IConventionFinder conventionFinder)
         {
            var steps = GetAdditionalMappingSteps();
            var basicSteps = new List<IAutomappingStep>
                {
                    new IdentityStep(this),
                    new VersionStep(this),
                    new ComponentStep(this),
                    new CustomPropertyStep(conventionFinder, this), //support overrides
                    new CustomHasManyToManyStep(this),
                    new ReferenceStep(this),
                    new CustomHasManyStep(this)
                };
            steps.AddRange(basicSteps);
            return steps;
        }

        private List<IAutomappingStep> GetAdditionalMappingSteps()
        {
            var stepTypes = _mappingStepsAssembiles.SelectMany(o => o.GetTypes().Where(t => typeof(IAutomappingStep).IsAssignableFrom(t)));
            var result = new List<IAutomappingStep>();
            foreach (var constuctor in stepTypes.Select(stepType => stepType.GetConstructor(new[] { typeof(IAutomappingConfiguration) })))
            {
                if (constuctor == null)
                    throw new NullReferenceException("constuctor");
                var mappingStep = (IAutomappingStep) constuctor.Invoke(new object[] {this});
                result.Add(mappingStep);
            }
            return result;
        }
    }
}
