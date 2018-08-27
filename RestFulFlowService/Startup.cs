using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RestFulFlowService.Services;
using SharedInterfaces.Interfaces.ServiceFarm;
using SharedServices.Services.ServiceFarm;
using SharedInterfaces.Interfaces.Proxy;
using SharedServices.Services.Proxy;

namespace RestFulFlowService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //TODO: Register IServiceFarmLoadBalancer as a singleton
            //TODO: Register IClientProxy as a Transient
            services.AddSingleton<IServiceFarmLoadBalancer, ServiceFarmLoadBalancer>();
            services.AddTransient<IClientProxy, ClientProxy>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            
            app.MapWhen(
                    //TODO: Use logical OR to Add multiple service routes that all get served up by the same middleware.
                    context => context.Request.Path.ToString().EndsWith("/love"),
                    appBranch =>
                    {
                        appBranch.UseRoutingWebService();
                    }
                );             
        }
    }
}
