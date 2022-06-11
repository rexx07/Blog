using System.Text.RegularExpressions;
using Blog.Data;
using Blog.Data.Data;
using Blog.Data.Domain.Articles;
using Blog.Services.Articles.Interfaces;
using Blog.Services.Extensions;
using Blog.Services.Models;
using Blog.Services.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Blog.Services.Articles.Services;

public class ArticleService : IArticleService
{
    private readonly ICategoryService _categoryService;
    private readonly IConfiguration _configuration;
    private readonly BlogDbContext _context;

    public ArticleService(BlogDbContext dbContext, ICategoryService categoryService, IConfiguration configuration)
    {
        _context = dbContext;
        _categoryService = categoryService;
        _configuration = configuration;
    }

    public async Task<List<Article>> GetArticles(PublishedStatus filter, ArticleType articletype)
    {
        switch (filter)
        {
            case PublishedStatus.Published:
                return await _context.Articles.AsNoTracking().Where(a => a.ArticleType == articletype)
                    .Where(a => a.Published > DateTime.MinValue).OrderByDescending(a => a.Published)
                    .ToListAsync();
            case PublishedStatus.Drafts:
                return await _context.Articles.AsNoTracking().Where(a => a.ArticleType == articletype)
                    .Where(a => a.Published == DateTime.MinValue).OrderByDescending(a => a.Id)
                    .ToListAsync();
            case PublishedStatus.Featured:
                return await _context.Articles.AsNoTracking().Where(a => a.ArticleType == articletype)
                    .Where(a => a.IsFeatured).OrderByDescending(a => a.Id).ToListAsync();
            default:
                return await _context.Articles.AsNoTracking().Where(a => a.ArticleType == articletype)
                    .OrderByDescending(a => a.Id).ToListAsync();
        }
    }

    public async Task<List<Article>> SearchArticles(string term)
    {
        if (term == "")
            return await _context.Articles.ToListAsync();

        return await _context.Articles.AsNoTracking().Where(a => a.Title.ToLower().Contains(term.ToLower()))
            .ToListAsync();
    }

    public async Task<Article> GetArticleById(int id)
    {
        return await _context.Articles.Where(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Article> GetArticleslug(string slug)
    {
        return await _context.Articles.Where(a => a.Slug == slug).FirstOrDefaultAsync();
    }

    public async Task<string> GetSlugFromTitle(string title)
    {
        var slug = title.ToSlug();
        var article = _context.Articles.FirstOrDefault(a => a.Slug == slug);

        if (article != null)
            for (var i = 2; i < 100; i++)
            {
                slug = $"(slug){i}";
                if (_context.Articles.FirstOrDefault(a => a.Slug == slug) == null) return await Task.FromResult(slug);
            }

        return await Task.FromResult(slug);
    }

    public async Task<bool> AddArticle(Article newArticle)
    {
        var existing = await _context.Articles.Where(a => a.Slug == newArticle.Slug).FirstOrDefaultAsync();
        if (existing != null)
            return false;

        newArticle.DateCreated = DateTime.UtcNow;
        newArticle.Content = newArticle.Content.RemoveScriptTags();
        newArticle.Content = newArticle.Description.RemoveScriptTags();

        await _context.Articles.AddAsync(newArticle);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateArticle(Article newArticle)
    {
        var existing = await _context.Articles.Where(a => a.Slug == newArticle.Slug).FirstOrDefaultAsync();
        if (existing == null)
            return false;

        existing.Slug = newArticle.Slug;
        existing.Title = newArticle.Title;
        existing.Description = newArticle.Description.RemoveScriptTags();
        existing.Content = newArticle.Content.RemoveScriptTags();
        existing.Cover = newArticle.Cover;
        existing.ArticleType = newArticle.ArticleType;
        existing.Published = newArticle.Published;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Publish(int id, bool publish)
    {
        var existing = await _context.Articles.Where(a => a.Id == id).FirstOrDefaultAsync();
        if (existing != null)
            return false;

        existing.Published = publish ? DateTime.Now : DateTime.MinValue;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> Featured(int id, bool featured)
    {
        var existing = await _context.Articles.Where(a => a.Id == id).FirstOrDefaultAsync();
        if (existing == null)
            return false;

        existing.IsFeatured = featured;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<ArticleItem>> GetArticleItems()
    {
        var articles = await _context.Articles.ToListAsync();
        var articleItems = new List<ArticleItem>();

        foreach (var article in articles)
            articleItems.Add(new ArticleItem
            {
                Id = article.Id,
                Title = article.Title,
                Description = article.Description,
                Content = article.Content,
                Slug = article.Slug,
                User = _context.Users.First(u => u.Id == article.UserId),
                Cover = string.IsNullOrEmpty(article.Cover) ? Constants.DefaultCover : article.Cover,
                Published = article.Published,
                ArticleViews = article.ArticleViews,
                Featured = article.IsFeatured
            });

        return articleItems;
    }

    public async Task<ArticleModel> GetArticleModel(string slug)
    {
        var model = new ArticleModel();
        var all = _context.Articles.AsNoTracking().Include(a => a.ArticleCategories)
            .OrderByDescending(a => a.IsFeatured).ThenByDescending(a => a.Published).ToList();

        await SetOlderNewerArticles(slug, model, all);

        var article = _context.Articles.Single(a => a.Slug == slug);
        article.ArticleViews++;
        await _context.SaveChangesAsync();

        model.Related = await Search(new Pager(1), model.Article.Title, 0, "PF");
        model.Related = model.Related.Where(r => r.Id != model.Article.Id).ToList();

        return await Task.FromResult(model);
    }

    public async Task<IEnumerable<ArticleItem>> GetPopular(Pager pager, int user = 0)
    {
        var skip = pager.CurrentPage * pager.ItemsPerPage - pager.ItemsPerPage;
        var articles = new List<Article>();

        if (user > 0)
            articles = _context.Articles.AsNoTracking().Where(a => a.Published > DateTime.MinValue && a.UserId == user)
                .OrderByDescending(a => a.ArticleViews).ThenByDescending(a => a.Published).ToList();
        else
            articles = _context.Articles.AsNoTracking().Where(a => a.Published > DateTime.MinValue)
                .OrderByDescending(a => a.ArticleViews).ThenByDescending(a => a.Published).ToList();

        pager.Configure(articles.Count);

        var items = new List<ArticleItem>();
        foreach (var a in articles.Skip(skip).Take(pager.ItemsPerPage).ToList())
            items.Add(await ArticleToItem(a, true));

        return await Task.FromResult(items);
    }

    public async Task<IEnumerable<ArticleItem>> Search(Pager pager, string term, int user = 0, string include = "",
        bool sanitize = true)
    {
        term = term.ToLower();
        var skip = pager.CurrentPage * pager.ItemsPerPage - pager.ItemsPerPage;

        var results = new List<SearchResult>();
        var termList = term.ToLower().Split(' ').ToList();
        var categories = await _context.Categories.ToListAsync();

        foreach (var a in GetArticles(include, user))
        {
            var rank = 0;
            var hits = 0;

            foreach (var termItem in termList)
            {
                if (termItem.Length < 4 && rank > 0) continue;

                if (a.ArticleCategories != null && a.ArticleCategories.Count > 0)
                    foreach (var ac in a.ArticleCategories)
                        if (ac.Category.Content.ToLower() == termItem)
                            rank += 10;

                if (a.Title.ToLower().Contains(termItem))
                {
                    hits = Regex.Matches(a.Title.ToLower(), termItem).Count;
                    rank += hits * 10;
                }

                if (a.Description.ToLower().Contains(termItem))
                {
                    hits = Regex.Matches(a.Title.ToLower(), termItem).Count;
                    rank += hits * 3;
                }

                if (a.Content.ToLower().Contains(termItem)) rank += Regex.Matches(a.Content.ToLower(), termItem).Count;
            }

            if (rank > 0) results.Add(new SearchResult { Rank = rank, Item = await ArticleToItem(a, sanitize) });
        }

        results = results.OrderByDescending(r => r.Rank).ToList();

        var articles = new List<ArticleItem>();
        for (var i = 0; i < results.Count; i++) articles.Add(results[i].Item);
        pager.Configure(articles.Count);
        return await Task.Run(() => articles.Skip(skip).Take(pager.ItemsPerPage).ToList());
    }

    public async Task<IEnumerable<ArticleItem>> GetList(Pager pager, int user = 0, string category = "",
        string include = "", bool sanitize = true)
    {
        var skip = pager.CurrentPage * pager.ItemsPerPage - pager.ItemsPerPage;

        var articles = new List<Article>();
        foreach (var a in GetArticles(include, user))
            if (string.IsNullOrEmpty(category))
            {
                articles.Add(a);
            }
            else
            {
                if (a.ArticleCategories != null && a.ArticleCategories.Count > 0)
                {
                    var cat = _context.Categories.Single(c => c.Content.ToLower() == category.ToLower());
                    if (cat == null)
                        continue;
                    foreach (var ac in a.ArticleCategories)
                        if (ac.CategoryId == cat.Id)
                            articles.Add(a);
                }
            }

        pager.Configure(articles.Count);

        var items = new List<ArticleItem>();
        foreach (var a in articles.Skip(skip).Take(pager.ItemsPerPage).ToList())
            items.Add(await ArticleToItem(a, sanitize));

        return await Task.FromResult(items);
    }

    public async Task<bool> Remove(int id)
    {
        var existing = await _context.Articles.Where(a => a.Id == id).FirstOrDefaultAsync();
        if (existing == null)
            return false;

        _context.Articles.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task SetOlderNewerArticles(string slug, ArticleModel model, List<Article> all)
    {
        if (all != null && all.Count > 0)
            for (var i = 0; i < all.Count; i++)
                if (all[i].Slug == slug)
                {
                    model.Article = await ArticleToItem(all[i]);

                    if (i > 0 && all[i - 1].Published > DateTime.MinValue)
                        model.Newer = await ArticleToItem(all[i - 1]);

                    if (i + 1 < all.Count && all[i + 1].Published > DateTime.MinValue)
                        model.Older = await ArticleToItem(all[i + 1]);

                    break;
                }
    }

    private async Task<ArticleItem> ArticleToItem(Article a, bool sanitize = false)
    {
        var article = new ArticleItem
        {
            Id = a.Id,
            ArticleType = a.ArticleType,
            Slug = a.Slug,
            Title = a.Title,
            Description = a.Description,
            Content = a.Content,
            Categories = await _categoryService.GetArticleCategories(a.Id),
            Cover = a.Cover,
            ArticleViews = a.ArticleViews,
            Rating = a.Rating,
            Published = a.Published,
            Featured = a.IsFeatured,
            User = _context.Users.Single(u => u.Id == a.UserId),
            SocialFields = new List<SocialField>()
        };

        if (article.User != null)
        {
            if (string.IsNullOrEmpty(article.User.Avatar))
                string.Format(Constants.AvatarDataImage, article.User.DisplayName.Substring(0, 1).ToUpper());

            article.User.Email = sanitize ? "donotreply@us.com" : article.User.Email;
        }

        return await Task.FromResult(article);
    }

    private List<Article> GetArticles(string include, int user)
    {
        var items = new List<Article>();
        var pubfeatured = new List<Article>();

        if (include.ToUpper().Contains(Constants.ArticleDraft) || string.IsNullOrEmpty(include))
        {
            var drafts = user > 0
                ? _context.Articles.Include(a => a.ArticleCategories).Where(a =>
                        a.Published == DateTime.MinValue && a.UserId == user && a.ArticleType == ArticleType.Article)
                    .ToList()
                : _context.Articles.Include(a => a.ArticleCategories).Where(a =>
                    a.Published == DateTime.MinValue && a.ArticleType == ArticleType.Article).ToList();
            items = items.Concat(drafts).ToList();
        }

        if (include.ToUpper().Contains(Constants.ArticleFeatured) || string.IsNullOrEmpty(include))
        {
            var featured = user > 0
                ? _context.Articles.Include(a => a.ArticleCategories).Where(a =>
                        a.Published > DateTime.MinValue && a.IsFeatured && a.UserId == user &&
                        a.ArticleType == ArticleType.Article)
                    .OrderByDescending(a => a.Published).ToList()
                : _context.Articles.Include(a => a.ArticleCategories).Where(a =>
                        a.Published > DateTime.MinValue && a.IsFeatured && a.ArticleType == ArticleType.Article)
                    .OrderByDescending(a => a.Published).ToList();
            pubfeatured = pubfeatured.Concat(featured).ToList();
        }

        if (include.ToUpper().Contains(Constants.ArticlePublished) || string.IsNullOrEmpty(include))
        {
            var published = user > 0
                ? _context.Articles.Include(a => a.ArticleCategories).Where(a =>
                    a.Published > DateTime.MinValue && !a.IsFeatured && a.UserId == user && a.ArticleType ==
                    ArticleType.Article).OrderByDescending(a => a.Published).ToList()
                : _context.Articles.Include(a => a.ArticleCategories).Where(a =>
                        a.Published > DateTime.MinValue && !a.IsFeatured && a.ArticleType == ArticleType.Article)
                    .OrderByDescending(a => a.Published).ToList();
            pubfeatured = pubfeatured.Concat(published).ToList();
        }

        pubfeatured = pubfeatured.OrderByDescending(p => p.Published).ToList();
        items = items.Concat(pubfeatured).ToList();

        return items;
    }
}