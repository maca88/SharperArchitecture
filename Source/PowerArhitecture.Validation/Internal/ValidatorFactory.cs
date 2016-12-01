using System;
using FluentValidation;
using SimpleInjector;

namespace PowerArhitecture.Validation.Internal
{
    /// <summary>
    /// Validation factory that uses an instance provider to create validators  
    /// </summary>
    internal class ValidatorFactory : ValidatorFactoryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorFactory"/> class.
        /// </summary>
        /// <param name="container">The IoC container.</param>
        public ValidatorFactory(Container container)
        {
            Container = container;
        }

        /// <summary>
        /// Gets or sets the IoC Container.
        /// </summary>
        /// <value>The IoC container.</value>
        public Container Container { get; set; }

        /// <summary>
        /// Creates an instance of a validator with the given type using ninject.
        /// </summary>
        /// <param name="validatorType">Type of the validator.</param>
        /// <returns>The newly created validator</returns>
        public override IValidator CreateInstance(Type validatorType)
        {
            return Container.GetInstance(validatorType) as IValidator;
        }
    }
}
