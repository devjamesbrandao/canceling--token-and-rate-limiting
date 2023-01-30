using Autentication.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Autentication.Infrastructure.Persistence.Context
{
    public class AutenticationContext : DbContext
    {
        public AutenticationContext(DbContextOptions<AutenticationContext> options)
            : base(options) {}

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AutenticationContext).Assembly);

            modelBuilder.Entity<User>().HasData(
                new User(){ Id = 1, Username = "Snape", Password = "Lilian" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}