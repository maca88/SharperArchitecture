using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Ninject.Activation;
using Ninject.Extensions.NamedScope;
using Ninject.Syntax;
using Ninject.Web.Common;

namespace PowerArhitecture.Common.Extensions
{
    public static class NinjectExtensions
    {
        public static IBindingInNamedWithOrOnSyntax<T> WhenAnyAncestorOrCurrentNamed<T>(this IBindingWhenSyntax<T> binding, string name)
        {
            return binding.WhenAnyAncestorMatches(ctx => ctx.AncestorOrCurrentNamed(name));
        }

        public static IBindingInNamedWithOrOnSyntax<T> WhenNoAncestorOrCurrentNamed<T>(this IBindingWhenSyntax<T> binding, string name)
        {
            return binding.WhenNoAncestorMatches(ctx => ctx.AncestorOrCurrentNamed(name));
        }

        public static bool IsAnyAncestorOrCurrentNamed(this IContext ctx, string name)
        {
            return DoesAnyAncestorMatch(ctx.Request, context => context.AncestorOrCurrentNamed(name));
        }

        public static IBindingInNamedWithOrOnSyntax<T> WhenRequestScopeExistsAndNoAncestorOrCurrentNamed<T>(this IBindingWhenSyntax<T> binding, string name)
        {
            return binding.When(request => request.ExistsRequestScope() && !DoesAnyAncestorMatch(request, context => context.AncestorOrCurrentNamed(name)));
        }

        public static IBindingInNamedWithOrOnSyntax<T> WhenRequestScopeNotExistsAndNoAncestorOrCurrentNamed<T>(this IBindingWhenSyntax<T> binding, string name)
        {
            return binding.When(request => !request.ExistsRequestScope() && !DoesAnyAncestorMatch(request, context => context.AncestorOrCurrentNamed(name)));
        }

        public static bool ExistsRequestScope(this IRequest request)
        {
            if (request.ParentContext != null)
            {
                return request.ParentContext.ExistsRequestScope();
            }
            return HttpContext.Current != null; //We dont have IContext to check the real scope implementation of INinjectHttpApplicationPlugin
        }

        public static bool ExistsRequestScope(this IContext context)
        {
            return context.Kernel.Components.GetAll<INinjectHttpApplicationPlugin>()
                    .Select(c => c.GetRequestScope(context))
                    .Any(s => s != null);
        }

        private static bool AncestorOrCurrentNamed(this IContext ctx, string name)
        {
            return ctx.Binding.Metadata.Name == name || ctx.Parameters.OfType<NamedScopeParameter>().Any(o => o.Name == name);
        }


        private static bool DoesAnyAncestorMatch(IRequest request, Predicate<IContext> predicate)
        {
            var parentContext = request.ParentContext;
            if (parentContext == null)
            {
                return false;
            }

            return
                predicate(parentContext) ||
                DoesAnyAncestorMatch(parentContext.Request, predicate);
        }
    }
}
