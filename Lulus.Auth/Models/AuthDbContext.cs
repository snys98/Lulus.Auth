using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lulus.Auth.Models
{
    public class AuthDbContext : IdentityDbContext<User>
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
            modelBuilder.Entity<Role>().ToTable("AspNetRoles");
            base.OnModelCreating(modelBuilder);
        }
    }
}
