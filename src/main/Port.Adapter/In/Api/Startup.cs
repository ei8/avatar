using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Owin;
using System;
using ei8.Avatar.Port.Adapter.Common;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using IdentityServer4.AccessTokenValidation;
using System.Net.Http;
using Microsoft.IdentityModel.Logging;

namespace ei8.Avatar.Port.Adapter.In.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddAuthorization();

            services.AddAuthentication(
                IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.TokenIssuerAddress);
                    // TODO: necessary?
                    //options.RequireHttpsMetadata = false;
                    //options.ApiSecret = "secret";
                    options.ApiName = "avatarapi";
                    // TODO: REMOVE ONCE CERTIFICATE SORTED
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    options.JwtBackChannelHandler = handler;
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            // TODO: 
            // app.UseHttpsRedirection();
            // app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseOwin(buildFunc => buildFunc.UseNancy());
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                var result = JsonConvert.SerializeObject(new { error = exception.Message });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }));
        }
    }
}
