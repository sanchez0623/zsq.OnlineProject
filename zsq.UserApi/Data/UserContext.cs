using Microsoft.EntityFrameworkCore;
using zsq.UserApi.Models;

namespace zsq.UserApi.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppUser>()
                .ToTable("Users")
                .HasKey(k => k.Id);

            builder.Entity<UserProperty>()
                .ToTable("UserProperties")
                .HasKey(k => new { k.Key, k.Text, k.Value });
            builder.Entity<UserProperty>()
                .Property(u => u.Key).HasMaxLength(100);
            builder.Entity<UserProperty>()
                .Property(u => u.Value).HasMaxLength(100);
            builder.Entity<UserProperty>()
                .Property(u => u.Text).HasMaxLength(100);

            builder.Entity<UserTag>()
                .ToTable("UserTags")
                .HasKey(k => new { k.AppUserId, k.Tag });
            builder.Entity<UserTag>()
                .Property(u => u.Tag)
                .HasMaxLength(100);

            builder.Entity<BPFile>()
                .ToTable("UserBPFiles")
                .HasKey(k => k.Id);

            base.OnModelCreating(builder);
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserProperty> UserProperties { get; set; }
        public DbSet<UserTag> UserTags { get; set; }
        public DbSet<BPFile> UserBPFiles { get; set; }
    }
}
