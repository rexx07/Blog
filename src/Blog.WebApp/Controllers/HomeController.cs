using System.Diagnostics;
using Blog.Data.Domain.Articles;
using Blog.Services.Articles.Interfaces;
using Blog.Services.Models;
using Blog.Services.Users.Interfaces;
using Blog.Services.Utils.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Blog.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly IArticleService _articleService;
    private readonly ICompositeViewEngine _compositeViewEngine;
    private readonly ILogger<HomeController> _logger;
    private readonly IStorageService _storageService;
    private readonly IUserService _userService;

    public HomeController(ILogger<HomeController> logger, IArticleService articleService, IUserService userService,
        IStorageService storageService, ICompositeViewEngine compositeViewEngine)
    {
        _logger = logger;
        _articleService = articleService;
        _userService = userService;
        _storageService = storageService;
        _compositeViewEngine = compositeViewEngine;
    }

    [HttpGet]
    public IEnumerable<Article> Get()
    {
        throw new NotImplementedException();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new Error { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}