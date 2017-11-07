using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace Lulus.Auth.DataSource.DbEntities
{
    public class RegisteredClientEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<string> AllowedGrantTypes { get; set; }
        public ICollection<string> RedirectUris { get; set; }
        public ICollection<string> PostLogoutRedirectUris { get; set; }
        public ICollection<string> AllowedScopes { get; set; }
        public ICollection<Secret> ClientSecrets { get; set; }
    }
}
