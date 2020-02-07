using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Owin;
using System;
using works.ei8.Cortex.Sentry.Port.Adapter.Common;

namespace works.ei8.Cortex.Sentry.Port.Adapter.In.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.TokenIssuerAddress);
                    options.RequireHttpsMetadata = false;
                    options.ApiSecret = "secret";
                    options.ApiName = "cortex-sentry";
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseOwin(buildFunc => buildFunc.UseNancy());
        }
    }
}
