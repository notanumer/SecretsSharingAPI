using Microsoft.EntityFrameworkCore;
using SecretsSharingAPI.Models;

namespace SecretsSharingAPI.EF
{
    public class UserContext : DbContext
    {
        private readonly DalSetting _setting;

        public DbSet<User> Users { get; set; }

        public DbSet<Models.File> Files { get; set; }

        public UserContext(DalSetting setting)
        {
            _setting = setting;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_setting.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.File>()
                .HasOne(owner => owner.User)
                .WithMany(u => u.Files)
                .HasForeignKey(fkey => fkey.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
