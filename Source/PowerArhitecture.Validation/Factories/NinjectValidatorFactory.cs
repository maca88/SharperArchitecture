using System;
using System.Collections.Generic;
using System.Linq;
using PowerArhitecture.Validation.Specifications;
using FluentValidation;
using Ninject;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace PowerArhitecture.Validation.Factories
{
    /// <summary>
    /// Validation factory that uses ninject to create validators  
    /// </summary>
    public class NinjectValidatorFactory : ValidatorFactoryBase, IValidatorFactoryExtended
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectValidatorFactory"/> class.
        /// </summary>
        /// <param name="resolutionRoot">The resolutionRoot.</param>
        public NinjectValidatorFactory(IResolutionRoot resolutionRoot)
        {
            ResolutionRoot = resolutionRoot;
        }

        /// <summary>
        /// Gets or sets the ResolutionRoot.
        /// </summary>
        /// <value>The kernel.</value>
        public IResolutionRoot ResolutionRoot
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an instance of a validator with the given type using ninject.
        /// </summary>
        /// <param name="validatorType">Type of the validator.</param>
        /// <returns>The newly created validator</returns>
        public override IValidator CreateInstance(Type validatorType)
        {
            /*
            if (((IList<IBinding>)Kernel.GetBindings(validatorType)).Count == 0)
            {
                return null;
            }*/
            return ResolutionRoot.Get(validatorType) as IValidator;
        }

        public IDictionary<Type, IValidator> GetValidators(IEnumerable<Type> types)
        {
            return types
                .ToDictionary(o => o, o => ResolutionRoot.Get(typeof(IValidator<>).MakeGenericType(o)) as IValidator);
        }
    }
}
