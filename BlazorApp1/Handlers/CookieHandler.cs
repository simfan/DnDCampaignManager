using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace BlazorApp1.Handlers
{
    public class CookieHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAntiforgery _antiforgery;

        public CookieHandler(IHttpContextAccessor httpContextAccessor, IAntiforgery antiforgery)
        {
            _httpContextAccessor = httpContextAccessor;
            _antiforgery = antiforgery;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                // Get the cookies from the current HTTP context
                var cookies = httpContext.Request.Headers["Cookie"].ToString();

                if (!string.IsNullOrEmpty(cookies))
                {
                    request.Headers.Add("Cookie", cookies);
                }

                if(request.Method != HttpMethod.Get)
                {
                    var tokens = _antiforgery.GetAndStoreTokens(httpContext);
                    if (tokens.RequestToken != null)
                    {
                        request.Headers.Add("RequestVerificationToken", tokens.RequestToken);
                    }
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}