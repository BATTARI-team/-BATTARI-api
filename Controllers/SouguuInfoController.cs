using BATTARI_api.Models.DTO;
using BATTARI_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry;

namespace BATTARI_api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
[Authorize]
public class SouguuInfoController
(CallingService callingService, ISouguuService souguuService) : ControllerBase
{
    [HttpGet]
    public SouguuNotificationDto? GetSouguuInfo()
    {
        try
        {
            var transaction = SentrySdk.StartTransaction(
                new TransactionContext("GetSouguuInfo", "GetSouguuInfo"));
            var userIdStr =
                HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type ==
                                         "http://schemas.xmlsoap.org/ws/2005/05/" +
                                             "identity/claims/name")!.Value;
            int userId = int.Parse(userIdStr);
            var a = callingService.GetCall(userId);
            transaction.Finish();
            return a;
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
        }

        return null;
    }

    [HttpGet]
    public bool ClearSouguuIncredient()
    {
        int userId;
        try
        {
            userId = Int16.Parse((HttpContext.User.Claims.FirstOrDefault(
                c => c.Type ==
                     "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"))!
                                     .Value);
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            return false;
        }

        try
        {
            souguuService.RemoveMaterial(userId);
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            return false;
        }
        return true;
    }

    [HttpPut]
    public IActionResult CancelCall(CancelCallWebsocketDto dto)
    {

        int userId;
        try
        {
            userId = Int16.Parse((HttpContext.User.Claims.FirstOrDefault(
                c => c.Type ==
                     "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"))!
                                     .Value);
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            return NotFound();
        }
        return Ok();
    }
}
