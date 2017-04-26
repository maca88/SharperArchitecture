using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Steps;
using FluentNHibernate.MappingModel.ClassBased;

namespace SharperArchitecture.DataAccess.MappingSteps
{
    /// <summary>
    /// Copied from nhiberate with a minor modification
    /// </summary>
    public class CustomHasManyStep : IAutomappingStep
    {
        private readonly SimpleTypeCollectionStep _simpleTypeCollectionStepStep;
        private readonly CustomCollectionStep _collectionStep;
        private readonly IAutomappingConfiguration _cfg;

        public CustomHasManyStep(IAutomappingConfiguration cfg)
        {
            _simpleTypeCollectionStepStep = new SimpleTypeCollectionStep(cfg);
            _collectionStep = new CustomCollectionStep(cfg);
            _cfg = cfg;
        }

        public bool ShouldMap(Member member)
        {
            return _simpleTypeCollectionStepStep.ShouldMap(member) || _collectionStep.ShouldMap(member);
        }

        public void Map(ClassMappingBase classMap, Member member)
        {
            //Added additional contraint ShouldMap so now we are able to map a collection in a ignored base class within a derivered one
            if (member.DeclaringType != classMap.Type && _cfg.ShouldMap(member.DeclaringType))
                return;
            if (_simpleTypeCollectionStepStep.ShouldMap(member))
            {
                _simpleTypeCollectionStepStep.Map(classMap, member);
            }
            else
            {
                if (!_collectionStep.ShouldMap(member))
                    return;
                _collectionStep.Map(classMap, member);
            }
        }
    }
}
