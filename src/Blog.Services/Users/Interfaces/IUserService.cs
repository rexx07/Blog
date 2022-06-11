using Blog.Data.Domain.Users;
using Blog.Services.Models;

namespace Blog.Services.Users.Interfaces;

public interface IUserService
{
    Task<List<User>> GetUsers();
    Task<User> FindByEmail(string email);
    Task<bool> Verify(LoginModel model);
    Task<bool> Add(User user);
    Task<bool> Update(User user);
    Task<bool> ChangePassword(RegisterModel model);
    Task<bool> Remove(int id);
}