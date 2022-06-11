using Blog.Data.Domain.Articles;
using Blog.Services.Models;

namespace Blog.Services.Articles.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryItem>> Categories();
    Task<List<CategoryItem>> SearchCategories(string term);
    Task<Category> GetCategory(int categoryId);
    Task<ICollection<Category>> GetArticleCategories(int articleId);
    Task<bool> SaveCategory(Category category);
    Task<Category> SaveCategory(string tag);
    Task<bool> AddArticleCategory(int articleId, string tag);
    Task<bool> SaveArticleCategories(int articleId, List<Category> categories);
    Task<bool> RemoveCategory(int categoryId);
}