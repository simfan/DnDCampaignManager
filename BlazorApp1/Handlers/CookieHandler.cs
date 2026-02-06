using Microsoft.AspNetCore.Http;

namespace BlazorApp1.Handlers
{
    public class CookieHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}