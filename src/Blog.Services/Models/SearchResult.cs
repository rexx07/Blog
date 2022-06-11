using Blog.Data.Domain;
using Blog.Services.ViewModels;

namespace Blog.Services.Models;

public class SearchResult : BaseEntityViewModel
{
    public int Rank { get; set; }
    public ArticleItem Item { get; set; }
}