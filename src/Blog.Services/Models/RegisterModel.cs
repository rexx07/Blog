using Blog.Data.Domain;

namespace Blog.Services.Models;

public class RegisterModel : BaseEntityViewModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirm { get; set; }
}