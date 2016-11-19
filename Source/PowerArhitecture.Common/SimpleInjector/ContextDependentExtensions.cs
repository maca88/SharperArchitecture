﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;
using SimpleInjector.Advanced;

namespace SimpleInjector.Extensions
{
    [DebuggerDisplay(
        "DependencyContext (ServiceType: {ServiceType}, " +
        "ImplementationType: {ImplementationType})")]
    public class DependencyContext
    {
        internal static readonly DependencyContext Root = new DependencyContext();

        internal DependencyContext(Type serviceType, Type implementationType,
            ParameterInfo parameter)
        {
            this.ServiceType = serviceType;
            this.ImplementationType = implementationType;
            this.Parameter = parameter;
        }

        private DependencyContext() { }

        public Type ServiceType { get; }
        public Type ImplementationType { get; }
        public ParameterInfo Parameter { get; }
    }

    public static class ContextDependentExtensions
    {
        public static void RegisterWithContext<TService>(this Container container,
            Func<DependencyContext, TService> contextBasedFactory, Action<Registration> alterRegistration = null) where TService : class
        {
            if (contextBasedFactory == null)
                throw new ArgumentNullException(nameof(contextBasedFactory));

            Func<TService> rootFactory = () => contextBasedFactory(DependencyContext.Root);

            // Should always be transient!
            var registration = Lifestyle.Transient.CreateRegistration(rootFactory, container);
            alterRegistration?.Invoke(registration);

            container.AddRegistration(typeof(TService), registration);
            // Allow the Func<DependencyContext, TService> to be injected into parent types.
            container.ExpressionBuilding += (sender, e) => {
                if (e.RegisteredServiceType != typeof(TService))
                {
                    var rewriter = new DependencyContextRewriter
                    {
                        ServiceType = e.RegisteredServiceType,
                        ContextBasedFactory = contextBasedFactory,
                        RootFactory = rootFactory,
                        Expression = e.Expression
                    };

                    e.Expression = rewriter.Visit(e.Expression);
                }
            };
        }

        private sealed class DependencyContextRewriter : ExpressionVisitor
        {
            private int parameterIndex;

            internal Type ServiceType { get; set; }
            internal object ContextBasedFactory { get; set; }
            internal object RootFactory { get; set; }
            internal Expression Expression { get; set; }

            internal Type ImplementationType
            {
                get
                {
                    var expression = this.Expression as NewExpression;

                    if (expression == null)
                        return this.ServiceType;

                    return expression.Constructor.DeclaringType;
                }
            }

            internal ParameterInfo Parameter
            {
                get
                {
                    var expression = this.Expression as NewExpression;

                    if (expression == null)
                        return null;

                    return expression.Constructor.GetParameters()[parameterIndex];
                }
            }

            protected override Expression VisitNew(NewExpression node)
            {
                return node.Update(this.VisitArguments(node.Arguments));
            }

            private IEnumerable<Expression> VisitArguments(IList<Expression> arguments)
            {
                for (int index = 0; index < arguments.Count; index++)
                {
                    this.parameterIndex = index;
                    yield return this.Visit(arguments[index]);
                }
            }

            protected override Expression VisitInvocation(InvocationExpression node)
            {
                if (!this.IsRootedContextBasedFactory(node))
                    return base.VisitInvocation(node);

                return Expression.Invoke(
                    Expression.Constant(this.ContextBasedFactory),
                    Expression.Constant(
                        new DependencyContext(
                            this.ServiceType,
                            this.ImplementationType,
                            this.Parameter)));
            }

            private bool IsRootedContextBasedFactory(InvocationExpression node)
            {
                var expression = node.Expression as ConstantExpression;

                if (expression == null)
                    return false;

                return object.ReferenceEquals(expression.Value, this.RootFactory);
            }
        }
    }
}
