using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace IdentityWebApiCommon.Middlewares
{
    public class RequestResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseMiddleware> _logger;

        public RequestResponseMiddleware(RequestDelegate next, ILogger<RequestResponseMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            string requestBody = string.Empty;
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            _logger.LogInformation("HTTP Request | Method: {Method} | Path: {Path} | Body: {Body}",
                context.Request.Method, context.Request.Path, requestBody);

            var originalResponseBodyStream = context.Response.Body;

            using (var responseBodyMemoryStream = new MemoryStream())
            {
                context.Response.Body = responseBodyMemoryStream;

                await _next(context);
                context.Response.Body.Position = 0;
                string responseBody = await new StreamReader(context.Response.Body, Encoding.UTF8).ReadToEndAsync();
                context.Response.Body.Position = 0;

                _logger.LogInformation("HTTP Response | Path: {Path} | Status: {StatusCode} | Body: {Body}",
                    context.Request.Path, context.Response.StatusCode, responseBody);

                await responseBodyMemoryStream.CopyToAsync(originalResponseBodyStream);
            }
        }
    }
}