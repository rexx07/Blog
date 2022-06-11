namespace Blog.Data.Domain.Articles;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Approved { get; set; }
    public string Content { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }

    public IList<ArticleCategory> ArticleCategories { get; set; }
}