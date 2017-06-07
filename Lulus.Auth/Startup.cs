using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Lulus.Auth.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lulus.Auth
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.UseMemoryCache(new MemoryCache(new MemoryCacheOptions() { ExpirationScanFrequency = TimeSpan.FromHours(1) }));
            });

            services.AddIdentity<User, Role>(options =>
             {
                 options.Password.RequireUppercase = false;
                 options.Password.RequireNonAlphanumeric = false;
                 options.User.RequireUniqueEmail = true;
             }).AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer()
                .AddTemporarySigningCredential()
                .AddInMemoryApiResources(AuthConfig.ApiResources)
                .AddInMemoryClients(AuthConfig.Clients)
                .AddInMemoryIdentityResources(AuthConfig.IdentityResources);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIdentity();

            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            //{
            //    AuthenticationScheme = "oidc",
            //    SignInScheme = "Cookies",

            //    Authority = "http://localhost:5000",
            //    RequireHttpsMetadata = false,

            //    ClientId = "auth",
            //    SaveTokens = true
            //});

            app.UseDeveloperExceptionPage();

            app.UseIdentityServer();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });
        }
    }
}
