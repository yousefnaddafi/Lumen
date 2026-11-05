using Microsoft.AspNetCore.Http.Features;

namespace Lumen.Extensions
{
    public static class HttpContextExtensions
    {
        public static string? GetCircuitId(this HttpContext context)
        {
            var feature = context.Features.Get<IHttpConnectionFeature>();
            return feature?.ConnectionId;
        }
    }
}
