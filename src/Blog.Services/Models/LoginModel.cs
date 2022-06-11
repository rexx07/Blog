using Blog.Data.Domain;

namespace Blog.Services.Models;

public class LoginModel : BaseEntityViewModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}