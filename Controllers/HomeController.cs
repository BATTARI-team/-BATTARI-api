using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BATTARI_api.Models;
using BATTARI_api.Repository.Data;
using BATTARI_api.ViewModel;

namespace BATTARI_api.Controllers;

public class HomeController(ISouguuService souguuService, IConfiguration configuration) : Controller
{
    public IActionResult Index()
    {
        return View(new UserViewModel(new UserContext(configuration)));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult SouguuIncredients()
    {
        return View(new SouguuIncredientsViewModel(souguuService));
    }
}
