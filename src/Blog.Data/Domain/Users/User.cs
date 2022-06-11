using Blog.Data.Domain.Articles;

namespace Blog.Data.Domain.Users;

public class User : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Bio { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string JobTitle { get; set; }
    public string Avatar { get; set; }
    public string? City { get; set; }
    public string State { get; set; }
    public string? Country { get; set; }
    public bool IsAuthor { get; set; }
    public bool IsAdmin { get; set; }
    public string? TwitterUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? LinkedinUrl { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }

    public List<Article>? Articles { get; set; }
    public List<Comment>? Comments { get; set; }
}