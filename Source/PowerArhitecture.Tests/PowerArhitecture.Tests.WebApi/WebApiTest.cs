using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate.Stat;
using NUnit.Framework;
using PowerArhitecture.Tests.WebApi.Server;
using PowerArhitecture.Tests.WebApi.Server.Controllers;

namespace PowerArhitecture.Tests.WebApi
{
    [TestFixture]
    public abstract class WebApiTest<TStartup>
    {
        private IDisposable _app;
        protected HttpClient HttpClient;

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            var port = FreeTcpPort();
            HttpClient = new HttpClient { BaseAddress = new Uri("http://localhost:" + port) };
            _app = WebApp.Start<TStartup>("http://localhost:" + port);
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            _app.Dispose();
        }

        protected Task<DatabaseStatistics> GetDatabaseStatistics(string dbConfigName = null)
        {
            return CallMethod<DatabaseController, DatabaseStatistics>(o => o.GetStatistics(dbConfigName));
        }

        protected Task ClearDatabaseStatistics(string dbConfigName = null)
        {
            return CallMethod<DatabaseController>(o => o.ClearStatistics(dbConfigName));
        }

        protected async Task CallMethod<T>(Expression<Action<T>> methodExpr)
            where T : ApiController
        {
            var methodCallExpression = (MethodCallExpression)methodExpr.Body;
            await CallMethod<object>(methodCallExpression);
        }

        protected Task<TResult> CallMethod<T, TResult>(Expression<Func<T, object>> methodExpr) where T : ApiController
        {
            var expression = StripConvert(methodExpr);
            var methodCallExpression = (MethodCallExpression)expression.Body;
            return CallMethod<TResult>(methodCallExpression);
        }

        private static int FreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        private async Task<TResult> CallMethod<TResult>(MethodCallExpression methodCallExpression)
        {
            var action = methodCallExpression.Method.Name;
            var controller = methodCallExpression.Method.DeclaringType.Name.Replace("Controller", "");

            var args = ResolveArgs(methodCallExpression);

            var i = 0;
            var paramDict = new Dictionary<string, string>();
            foreach (var param in methodCallExpression.Method.GetParameters())
            {
                if (args[i].Value != null)
                {
                    paramDict.Add(param.Name, args[i].Value?.ToString());
                }
                i++;
            }

            var get = methodCallExpression.Method.GetCustomAttribute<HttpGetAttribute>();
            var post = methodCallExpression.Method.GetCustomAttribute<HttpPostAttribute>();

            HttpMethod method;

            if (get == null && post == null)
            {
                method = action.StartsWith("Get") ? HttpMethod.Get : HttpMethod.Post;
            }
            else if (get != null)
            {
                method = HttpMethod.Get;
            }
            else
            {
                method = HttpMethod.Post;
            }

            var url = $"/{controller}/{action}";

            HttpResponseMessage response;
            if (method == HttpMethod.Get)
            {
                response = await HttpClient.GetAsync(url + GetQueryString(paramDict));
                response.EnsureSuccessStatusCode();
            }
            else
            {
                response = await HttpClient.PostAsync(url, new FormUrlEncodedContent(paramDict));
                response.EnsureSuccessStatusCode();
            }

            var returnType = methodCallExpression.Method.ReturnType;
            if (returnType == typeof(void))
            {
                return default(TResult);
            }
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResult>(result);
        }

        private static string GetQueryString(Dictionary<string, string> nvc)
        {
            if (nvc.Count == 0)
            {
                return "";
            }
            return "?" + string.Join("&", nvc.Select(o => $"{HttpUtility.UrlEncode(o.Key)}={HttpUtility.UrlEncode(o.Value)}"));
        }

        private static LambdaExpression StripConvert<T>(Expression<Func<T, object>> source)
        {
            var result = source.Body;
            // use a loop in case there are nested Convert expressions for some crazy reason
            while (((result.NodeType == ExpressionType.Convert)
                       || (result.NodeType == ExpressionType.ConvertChecked))
                   && (result.Type == typeof(object)))
            {
                result = ((UnaryExpression)result).Operand;
            }
            return Expression.Lambda(result, source.Parameters);
        }

        private static KeyValuePair<Type, object>[] ResolveArgs(MethodCallExpression body)
        {
            var values = new List<KeyValuePair<Type, object>>();

            foreach (var argument in body.Arguments)
            {
                var exp = ResolveMemberExpression(argument);
                var type = argument.Type;

                var value = GetValue(exp);

                values.Add(new KeyValuePair<Type, object>(type, value));
            }
            return values.ToArray();
        }

        private static MemberExpression ResolveMemberExpression(Expression expression)
        {

            if (expression is MemberExpression)
            {
                return (MemberExpression)expression;
            }
            else if (expression is UnaryExpression)
            {
                // if casting is involved, Expression is not x => x.FieldName but x => Convert(x.Fieldname)
                return (MemberExpression)((UnaryExpression)expression).Operand;
            }
            else
            {
                throw new NotSupportedException(expression.ToString());
            }
        }

        private static object GetValue(MemberExpression exp)
        {
            // expression is ConstantExpression or FieldExpression
            if (exp.Expression is ConstantExpression)
            {
                return (((ConstantExpression)exp.Expression).Value)
                        .GetType()
                        .GetField(exp.Member.Name)
                        .GetValue(((ConstantExpression)exp.Expression).Value);
            }
            else if (exp.Expression is MemberExpression)
            {
                return GetValue((MemberExpression)exp.Expression);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
