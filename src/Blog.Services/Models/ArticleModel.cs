using Blog.Services.ViewModels;

namespace Blog.Services.Models;

public class ArticleModel
{
    public ArticleItem Article { get; set; }
    public ArticleItem Older { get; set; }
    public ArticleItem Newer { get; set; }
    public IEnumerable<ArticleItem> Related { get; set; }
}