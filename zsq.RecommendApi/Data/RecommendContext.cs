using Microsoft.EntityFrameworkCore;
using zsq.RecommendApi.Models;

namespace zsq.RecommendApi.Data
{
    public class RecommendContext : DbContext
    {
        public DbSet<ProjectRecommend> ProjectRecommends { get; set; }

        public RecommendContext(DbContextOptions<RecommendContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectRecommend>().ToTable("ProjectRecommends").HasKey(r => r.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
