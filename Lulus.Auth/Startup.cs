using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Models;
using Lulus.Auth.DataSource;
using Lulus.Auth.DataSource.DbEntities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
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
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Lulus.Auth Doc",
                    Description = "RESTful API for Lulus.Auth",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Lulus", Email = "snys98@outlook.com", Url = "" }
                });
            });
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.UseMemoryCache(new MemoryCache(new MemoryCacheOptions()
                {
                    ExpirationScanFrequency = TimeSpan.FromMinutes(10),
                    SizeLimit = 134217728
                }));
            });
            services.AddIdentity<UserEntity, RoleEntity>(options =>
             {
                 options.Password.RequireUppercase = false;
                 options.Password.RequireNonAlphanumeric = false;
                 options.User.RequireUniqueEmail = true;
             }).AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

            //这里原来使用的inmemoryclient，修改为数据库存储
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddClientStore<AuthDbContext>()
                .AddResourceStore<AuthDbContext>()
                .AddInMemoryApiResources(AuthConfig.ApiResources)
                //.AddInMemoryClients(AuthConfig.Clients)
                .AddInMemoryIdentityResources(new []{ new IdentityResources.OpenId() });

            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 536870912;
                options.ExpirationScanFrequency = TimeSpan.FromHours(1);
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseAuthentication();
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
            app.UseSwagger();
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint("/swagger/v1/swagger.json", "Lulus.Auth API V1");
                option.ShowRequestHeaders();
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<RegisteredResourceEntity, Resource>()
                    .Include<RegisteredResourceEntity, IdentityResources.OpenId>()
                    .Include<RegisteredResourceEntity, ApiResource>();

                cfg.CreateMap<RegisteredClientEntity, Client>();
            });
        }
    }
}
