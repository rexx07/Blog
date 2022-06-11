using Blog.Data.Data;
using Blog.Data.Domain.Users;
using Blog.Services.Extensions;
using Blog.Services.Models;
using Blog.Services.Users.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Blog.Services.Users.Services;

public class UserService : IUserService
{
    private static string _salt;
    private readonly BlogDbContext _context;

    public UserService(BlogDbContext context, IConfiguration configuration)
    {
        _context = context;
        _salt = configuration.GetSection("Blugg:Salt").Value; //GetValue<string>("Salt")
    }

    public async Task<List<User>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> FindByEmail(string email)
    {
        return await Task.FromResult(_context.Users.FirstOrDefault(u => u.Email == email));
    }

    public async Task<bool> Verify(LoginModel model)
    {
        Log.Warning($"Verifying password for {model.Email}");

        var existing = await Task.FromResult(_context.Users.FirstOrDefault(u => u.Email == model.Email));
        if (existing == null)
        {
            Log.Warning($"User with email{model.Email} not found");
            return false;
        }

        if (existing.Password == model.Password.Hash(_salt))
        {
            Log.Warning($"Successful login for {model.Password}");
            return true;
        }

        Log.Warning("Password does not match");
        return false;
    }

    public async Task<bool> Add(User user)
    {
        var existing = await _context.Users.Where(u => u.Email == user.Email).OrderBy(u => u.Id).FirstOrDefaultAsync();
        if (existing == null)
            return false;

        user.IsAdmin = false;
        user.Password = user.Password.Hash(_salt);
        user.Avatar = string.Format(Constants.AvatarDataImage, user.DisplayName.Substring(0, 1).ToUpper());
        user.DateCreated = DateTime.UtcNow;

        _context.Users.Add(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Update(User user)
    {
        var existing = await _context.Users.Where(u => u.Email == user.Email).FirstOrDefaultAsync();
        if (existing == null)
            return false;

        if (existing.IsAdmin && !user.IsAdmin)
            // do not remove last admin account
            if (_context.Users.Count(a => a.IsAdmin) == 1)
                return false;

        existing.Email = user.Email;
        existing.DisplayName = user.DisplayName;
        existing.Bio = user.Bio;
        existing.Avatar = user.Avatar;
        existing.IsAdmin = user.IsAdmin;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ChangePassword(RegisterModel model)
    {
        var existing = await _context.Users.Where(u => u.Email == model.Email).FirstOrDefaultAsync();
        if (existing == null)
            return false;

        existing.Password = model.Password.Hash(_salt);

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Remove(int id)
    {
        var existingAuthor = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
        if (existingAuthor == null)
            return false;

        _context.Users.Remove(existingAuthor);
        await _context.SaveChangesAsync();
        return true;
    }
}