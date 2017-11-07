using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lulus.Auth.DataSource.DbEntities
{
    public class RegisteredResourceEntity
    {
        public ICollection<string> ScopeNames { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public bool Emphasize { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public ICollection<string> UserClaims { get; set; }
        public ResourceType ResourceType { get; set; }
    }

    public enum ResourceType
    {
        Identity,
        Api
    }
}
