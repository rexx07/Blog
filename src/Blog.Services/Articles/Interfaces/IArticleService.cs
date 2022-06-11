using Blog.Data;
using Blog.Data.Domain.Articles;
using Blog.Services.Models;
using Blog.Services.ViewModels;

namespace Blog.Services.Articles.Interfaces;

public interface IArticleService
{
    Task<List<Article>> GetArticles(PublishedStatus filter, ArticleType articletype);
    Task<List<Article>> SearchArticles(string term);
    Task<Article> GetArticleById(int id);
    Task<Article> GetArticleslug(string slug);
    Task<string> GetSlugFromTitle(string title);
    Task<bool> AddArticle(Article newArticle);
    Task<bool> UpdateArticle(Article newArticle);
    Task<bool> Publish(int id, bool publish);
    Task<bool> Featured(int id, bool featured);
    Task<IEnumerable<ArticleItem>> GetArticleItems();
    Task<ArticleModel> GetArticleModel(string slug);
    Task<IEnumerable<ArticleItem>> GetPopular(Pager pager, int user = 0);

    Task<IEnumerable<ArticleItem>> Search(Pager pager, string term, int user = 0,
        string include = "", bool sanitize = true);

    Task<IEnumerable<ArticleItem>> GetList(Pager pager, int user = 0, string category = "",
        string include = "", bool sanitize = true);

    Task<bool> Remove(int id);
}