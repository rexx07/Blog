using Blog.Data.Domain;

namespace Blog.Services.ViewModels;

public class CustomField : BaseEntityViewModel
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
}

public class SocialField : BaseEntityViewModel
{
    public string Title { get; set; }
    public string Icon { get; set; }
    public int Rank { get; set; }
}