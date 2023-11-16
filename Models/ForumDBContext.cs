using Microsoft.EntityFrameworkCore;

namespace Lab4_5.Models
{
    public class ForumDBContext : DbContext
    {
        public ForumDBContext(DbContextOptions<ForumDBContext> options) : base(options) 
        {

        }
        
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Topic> Topics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>()
                .HasOne(rw => rw.Topic)
                .WithMany(t => t.Reviews)
                .HasForeignKey(t => t.TopicId);
        }
    }
}
