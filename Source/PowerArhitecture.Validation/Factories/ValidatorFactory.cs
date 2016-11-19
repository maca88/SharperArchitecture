using System;
using System.Collections.Generic;
using System.Linq;
using PowerArhitecture.Validation.Specifications;
using FluentValidation;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Validation.Factories
{
    /// <summary>
    /// Validation factory that uses an instance provider to create validators  
    /// </summary>
    internal class ValidatorFactory : ValidatorFactoryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorFactory"/> class.
        /// </summary>
        /// <param name="instanceProvider">The instanceProvider.</param>
        public ValidatorFactory(IInstanceProvider instanceProvider)
        {
            InstanceProvider = instanceProvider;
        }

        /// <summary>
        /// Gets or sets the InstanceProvider.
        /// </summary>
        /// <value>The kernel.</value>
        public IInstanceProvider InstanceProvider { get; set; }

        /// <summary>
        /// Creates an instance of a validator with the given type using ninject.
        /// </summary>
        /// <param name="validatorType">Type of the validator.</param>
        /// <returns>The newly created validator</returns>
        public override IValidator CreateInstance(Type validatorType)
        {
            return InstanceProvider.Get(validatorType) as IValidator;
        }
    }
}
