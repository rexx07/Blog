namespace Blog.Data.Domain.Articles;

public class Newsletter : BaseEntity
{
    public int ArticleId { get; set; }
    public bool Success { get; set; }

    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }

    public Article Article { get; set; }
}