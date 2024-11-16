using System.Net;
using BATTARI_api.Models.DTO;
using BATTARI_api.Repository;
using BATTARI_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BATTARI_api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
[Authorize]
public class SouguuInfoController(CallingService callingService) : ControllerBase
{
    [HttpGet]
    public SouguuNotificationDto? GetSouguuInfo()
    {
        var userIdStr = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")!.Value;
        int userId = int.Parse(userIdStr);
        var a = callingService.GetCall(userId);
        return a;
    }
}