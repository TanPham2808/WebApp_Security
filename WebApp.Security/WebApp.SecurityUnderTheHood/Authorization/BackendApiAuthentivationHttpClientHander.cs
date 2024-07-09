
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApp.SecurityUnderTheHood.Authorization
{
    public class BackendApiAuthentivationHttpClientHander : DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _accessor;

        public BackendApiAuthentivationHttpClientHander(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor accessor)
        {
            _httpClientFactory = httpClientFactory;
            _accessor = accessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _accessor.HttpContext?.Request.Cookies["MyCookieAuth"];

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
