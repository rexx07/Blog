using Blog.Data.Domain.Users;

namespace Blog.Data.Domain.Articles;

/// <summary>
///     Article model
/// </summary>
public class Article : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }

    public string Title { get; set; }
    public string Cover { get; set; }
    public string Slug { get; set; }
    public string CoverCredit { get; set; }
    public List<string> Tags { get; set; }
    public string Content { get; set; }
    public string Description { get; set; }
    public ArticleType ArticleType { get; set; }
    public DateTime Published { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }

    public int ArticleViews { get; set; }
    public int CountWords { get; set; }
    public int ReadTime { get; set; }
    public double Rating { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsSelected { get; set; }

    public IList<ArticleCategory> ArticleCategories { get; set; }
    public IList<Comment> Comments { get; set; }
}