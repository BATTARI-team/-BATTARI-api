using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BATTARI_api.Models;
using BATTARI_api.Repository.Data;

namespace BATTARI_api.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View(new UserViewModel(new UserContext()));
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
}
