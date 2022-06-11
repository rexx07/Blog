using System.Diagnostics;
using Blog.Services.Articles.Interfaces;
using Blog.Services.Models;
using Blog.Services.Users.Interfaces;
using Blog.Services.Utils.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Blog.Web.Controllers;

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

    public async Task<IActionResult> Index()
    {
        //var model = await getBlogPosts(pager: page);

        //If no blogs are setup redirect to first time registration
        //if(model == null){
        //return Redirect("~/admin/register");
        // }

        return View(); //$"~/Views/Themes/{model.Blog.Theme}/Index.cshtml", model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new Error { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}