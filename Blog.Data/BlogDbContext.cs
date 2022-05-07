using Blog.Data.Domain.Articles;
using Blog.Data.Domain.Categories;
using Blog.Data.Domain.Comments;
using Blog.Data.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<User> UserProfiles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }


        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<User>().HasMany(u => u.Articles).WithOne(a => a.User).HasForeignKey(a => a.UserId)
                .HasPrincipalKey(u => u.Id);

            b.Entity<Article>().HasOne(a => a.User).WithMany(u => u.Articles).HasForeignKey(a => a.UserId)
                .HasPrincipalKey(a =>);
            b.Entity<Category>().HasKey(c => c.Id);
            b.Entity<Comment>().
        }
    }
}
