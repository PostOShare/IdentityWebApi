using IdentityWebApiCommon.Middlewares;

namespace IdentityWebApi
{
    public static class ModelValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseModelValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseMiddleware>();
        }
    }
}