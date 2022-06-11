using Blog.Data.Domain.Users;

namespace Blog.Data.Domain.Articles;

public class Comment : BaseEntity
{
    public Article Article { get; set; }
    public int ArticleId { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public bool Approved { get; set; }
}