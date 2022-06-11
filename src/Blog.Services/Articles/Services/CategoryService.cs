using Blog.Data.Data;
using Blog.Data.Domain.Articles;
using Blog.Services.Articles.Interfaces;
using Blog.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Services.Articles.Services;

public class CategoryService : ICategoryService
{
    private readonly BlogDbContext _context;

    public CategoryService(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryItem>> Categories()
    {
        var cats = new List<CategoryItem>();

        if (_context.Articles != null && _context.Articles.Any())
            foreach (var ac in _context.ArticleCategories.Include(ac => ac.Category).AsNoTracking())
                if (!cats.Exists(c => c.Category.ToLower() == ac.Category.Content.ToLower()))
                {
                    cats.Add(new CategoryItem
                    {
                        Selected = false,
                        Id = ac.CategoryId,
                        Category = ac.Category.Content.ToLower(),
                        ArticleCount = 1,
                        DateCreated = ac.Category.DateCreated
                    });
                }
                else
                {
                    var tmp = cats.FirstOrDefault(c => c.Category.ToLower() == ac.Category.Content.ToLower());
                    tmp.ArticleCount++;
                }

        return await Task.FromResult(cats);
    }

    public async Task<List<CategoryItem>> SearchCategories(string term)
    {
        var cats = await Categories();

        if (term == "*")
            return cats;

        return cats.Where(c => c.Category.ToLower().Contains(term.ToLower())).ToList();
    }

    public async Task<Category> GetCategory(int categoryId)
    {
        return await _context.Categories.Where(c => c.Id == categoryId).FirstOrDefaultAsync();
    }

    public async Task<ICollection<Category>> GetArticleCategories(int articleId)
    {
        return await _context.ArticleCategories.AsNoTracking().Where(ac => ac.ArticleId == articleId)
            .Select(ac => ac.Category).ToListAsync();
    }

    public async Task<bool> SaveCategory(Category category)
    {
        var contextCategory = await _context.Categories.Where(c => c.Id == category.Id).FirstOrDefaultAsync();

        if (contextCategory == null)
            return false;

        contextCategory.Content = category.Content;
        contextCategory.Description = category.Description;
        contextCategory.DateUpdated = DateTime.UtcNow;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Category> SaveCategory(string tag)
    {
        var category = await _context.Categories.AsNoTracking().Where(c => c.Content == tag)
            .FirstOrDefaultAsync();

        if (category == null)
            return category;

        category = new Category
        {
            Content = tag,
            DateCreated = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return category;
    }

    public async Task<bool> AddArticleCategory(int articleId, string tag)
    {
        var category = await SaveCategory(tag);

        if (category == null)
            return false;

        var article = await _context.Articles.Where(a => a.Id == articleId).FirstOrDefaultAsync();
        if (article == null)
            return false;

        var articleCategory = await _context.ArticleCategories.AsNoTracking().Where(ac =>
            ac.CategoryId == category.Id).Where(ac => ac.ArticleId == articleId).FirstOrDefaultAsync();
        if (articleCategory == null)
        {
            _context.ArticleCategories.Add(new ArticleCategory
            {
                CategoryId = category.Id,
                ArticleId = articleId
            });
            return await _context.SaveChangesAsync() > 0;
        }

        return false;
    }

    public async Task<bool> SaveArticleCategories(int articleId, List<Category> categories)
    {
        var existingArticleCategories = await _context.ArticleCategories.AsNoTracking()
            .Where(ac => ac.ArticleId == articleId).ToListAsync();

        _context.ArticleCategories.RemoveRange(existingArticleCategories);

        await _context.SaveChangesAsync();

        foreach (var cat in categories) await AddArticleCategory(articleId, cat.Content);

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveCategory(int categoryId)
    {
        var articleCategories = await _context.ArticleCategories.AsNoTracking()
            .Where(ac => ac.CategoryId == categoryId).ToListAsync();
        _context.ArticleCategories.RemoveRange(articleCategories);

        var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
        _context.Categories.Remove(category);

        return await _context.SaveChangesAsync() > 0;
    }
}