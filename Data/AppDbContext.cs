using BaseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.FullName);

                entity.Property(x => x.Email);

                entity.Property(x => x.PasswordHash);

                entity.Property(x => x.Role);

                entity.Property(x => x.IsActive);

            });
        }
    }
}
