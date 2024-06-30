using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
class AdminController : Controller {
    public IActionResult Index() {
        return View();
    }
}