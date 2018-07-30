using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestFulFlowService.Services
{
    public static class WebServicesExtensions
    {
        public static IApplicationBuilder UseRoutingWebService(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RoutingWebService>();
        }
    }
}
