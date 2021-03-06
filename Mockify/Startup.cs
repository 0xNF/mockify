﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspNet.Security.OpenIdConnect;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Bson;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using Mockify.Data;
using Mockify.Models;
using Mockify.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace Mockify {
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MockifyDbContext>(options =>
                //options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
                options.UseSqlite("Data Source=Mockify.db")
            );

            services.AddIdentity<ApplicationUser, IdentityRole>((x) => {
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequiredLength = 6;
                x.Password.RequireDigit = false;
                x.Password.RequireLowercase = false;
                x.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<MockifyDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IValidateAuthorizationService, ValidationAuthorizationService>();
            services.AddTransient<IRateLimitService, RateLimitService>();
            services.AddSingleton<IHostedService, RateLimitResetService>();



            services.AddAuthentication().AddOpenIdConnectServer(options => {
                options.Provider = new MockifyAuthorizationProvider();
                options.TokenEndpointPath = "/api/token";
                options.AuthorizationEndpointPath = "/authorize/";
                options.UseSlidingExpiration = false; // Spotify does not issue new refresh tokens
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }

    }
}
