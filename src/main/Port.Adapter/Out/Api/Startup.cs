﻿using Microsoft.AspNetCore.Builder;
using Nancy.Owin;

namespace works.ei8.Avatar.Port.Adapter.Out.Api
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(buildFunc => buildFunc.UseNancy());
        }
    }
}
