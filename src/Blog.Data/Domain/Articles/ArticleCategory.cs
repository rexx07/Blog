namespace Blog.Data.Domain.Articles;

public class ArticleCategory : BaseEntity
{
    public int ArticleId { get; set; }
    public Article Article { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }
}