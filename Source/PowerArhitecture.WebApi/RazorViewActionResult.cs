using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using RazorEngine;

namespace PowerArhitecture.WebApi
{
    public class RazorViewActionResult : IHttpActionResult
    {
        private readonly HttpRequestMessage _request;
        private readonly string _viewPath;
        private readonly object _model;

        public RazorViewActionResult(HttpRequestMessage request, string viewPath, object model = null)
        {
            _request = request;
            _viewPath = viewPath;
            _model = model;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = _request.CreateResponse(HttpStatusCode.Created);
            var viewPath = HttpContext.Current.Server.MapPath(_viewPath);
            var template = File.ReadAllText(viewPath);
            var html = Razor.Parse(template, _model);
            response.Content = new StringContent(html);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return Task.FromResult(response);
        }
    }
}
