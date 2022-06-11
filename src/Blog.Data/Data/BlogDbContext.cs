using Blog.Data.Domain.Articles;
using Blog.Data.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Data;

public class BlogDbContext : DbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Newsletter> Newsletters { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<ArticleCategory> ArticleCategories { get; set; }


    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().Property(u => u.Id);
        b.Entity<User>().HasMany(u => u.Comments);
        b.Entity<User>().HasMany(u => u.Articles);
        b.Entity<User>().HasKey(u => u.Id);

        b.Entity<Article>().Property(a => a.Id);
        b.Entity<Article>().HasKey(a => a.Id);
        b.Entity<Article>().HasOne(u => u.User).WithMany(a => a.Articles)
            .HasForeignKey(a => a.UserId);

        b.Entity<Category>().Property(c => c.Id);
        b.Entity<Category>().HasMany(c => c.ArticleCategories);

        b.Entity<Comment>().Property(c => c.Id);
        b.Entity<Comment>().HasOne(c => c.Article);
        b.Entity<Comment>().HasOne(c => c.User);
        b.Entity<Comment>().HasKey(c => c.Id);

        b.Entity<ArticleCategory>().Property(ac => ac.Id);
        b.Entity<ArticleCategory>()
            .HasKey(ac => new { ac.ArticleId, ac.CategoryId });
        b.Entity<ArticleCategory>()
            .HasOne(ac => ac.Article)
            .WithMany(a => a.ArticleCategories)
            .HasForeignKey(ac => ac.ArticleId);
        b.Entity<ArticleCategory>()
            .HasOne(ac => ac.Category)
            .WithMany(c => c.ArticleCategories)
            .HasForeignKey(ac => ac.CategoryId);

        b.Entity<Newsletter>().Property(ns => ns.Id);
    }
}