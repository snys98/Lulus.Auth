using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Lulus.Auth.DataSource.DbEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Lulus.Auth.DataSource
{
    public class AuthDbContext : IdentityDbContext<UserEntity>,IClientStore, IResourceStore
    {
        //public DbSet<User> Users { get; set; }
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleEntity>().ToTable("AspNetRoles");
            base.OnModelCreating(modelBuilder);
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {

            return Task.Run(async () =>
            {
                var entity = await this.Clients.FirstOrDefaultAsync(x => x.Id == clientId);
                return new Client()
                {
                    ClientId = entity.Id,
                    ClientName = entity.Name,
                    AllowedGrantTypes = entity.AllowedGrantTypes,
                    ClientSecrets = entity.ClientSecrets,
                    // where to redirect to after login
                    RedirectUris = entity.RedirectUris, //{ "http://localhost:5002/signin-oidc" },
                    // where to redirect to after logout
                    PostLogoutRedirectUris = entity.PostLogoutRedirectUris, //{ "http://localhost:5002/signout-callback-oidc" },
                    AllowedScopes = entity.AllowedScopes
                    //new List<string>
                    //{
                    //    IdentityServerConstants.StandardScopes.OpenId,
                    //    IdentityServerConstants.StandardScopes.Profile
                    //}
                };
            });
        }
        List<Client> _clientCache = new List<Client>();
        public DbSet<RegisteredClientEntity> Clients { get; set; }
        List<RegisteredResourceEntity> _resourceCache = new List<RegisteredResourceEntity>();
        public DbSet<RegisteredResourceEntity> Resources { get; set; }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return Task.Run(async () =>
            {
                //直接获取所有，startup中有memoryCache所以无所谓
                var resouces = await this.Resources.ToListAsync();
                var entities = resouces.Where(x => x.ScopeNames.Intersect(scopeNames).Any());
                return entities.Select(x => new IdentityResource()
                {
                    Description = x.Description,
                    DisplayName = x.DisplayName,
                    Emphasize = x.Emphasize,
                    Enabled = x.Enabled,
                    Name = x.Name,
                    Required = x.Required,
                    ShowInDiscoveryDocument = x.ShowInDiscoveryDocument,
                    UserClaims = x.UserClaims,
                });
            });
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return Task.Run(async () =>
            {
                //直接获取所有，startup中有memoryCache所以无所谓
                var resouces = await this.Resources.ToListAsync();
                var entities = resouces.Where(x => x.ScopeNames.Intersect(scopeNames).Any());
                //return Mapper.Map<ApiResource>(entities);
                return entities.Select(Mapper.Map<ApiResource>);
            });
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            return Task.Run(async () =>
            {
                //直接获取所有，startup中有memoryCache所以无所谓
                var resouces = await this.Resources.ToListAsync();
                var entity = resouces.FirstOrDefault(x => x.Name == name);
                if (entity == null)
                {
                    return null;
                }
                return Mapper.Map<ApiResource>(entity);
                //return new ApiResource()
                //{
                //    Description = x.Description,
                //    DisplayName = x.DisplayName,
                //    Emphasize = x.Emphasize,
                //    Enabled = x.Enabled,
                //    Name = x.Name,
                //    Required = x.Required,
                //    ShowInDiscoveryDocument = x.ShowInDiscoveryDocument,
                //    UserClaims = x.UserClaims,
                //};
            });
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            return Task.Run(async () =>
            {
                //直接获取所有，startup中有memoryCache所以无所谓
                var resouces = await this.Resources.ToListAsync();
                return new Resources(resouces.Where(x => x.ResourceType == ResourceType.Identity).Select(Mapper.Map<IdentityResource>), resouces.Where(x => x.ResourceType == ResourceType.Api).Select(Mapper.Map<ApiResource>));
            });
        }
    }

    
}
