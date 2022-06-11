using Blog.Data;
using Blog.Data.Domain;
using Blog.Data.Domain.Articles;
using Blog.Data.Domain.Users;

namespace Blog.Services.ViewModels;

public class ArticleItem : BaseEntityViewModel
{
    public ArticleType ArticleType { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public ICollection<Category> Categories { get; set; }
    public string Cover { get; set; }
    public int ArticleViews { get; set; }
    public double Rating { get; set; }
    public DateTime Published { get; set; }
    public bool IsPublished => Published > DateTime.MinValue;
    public bool Featured { get; set; }

    public User User { get; set; }
    public SaveStatus Status { get; set; }
    public List<SocialField> SocialFields { get; set; }
    public bool Selected { get; set; }

    // to be able compare two posts
    // if(post1 == post2) { ... }
    public bool Equals(ArticleItem other)
    {
        if (Id == other.Id)
            return true;

        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}