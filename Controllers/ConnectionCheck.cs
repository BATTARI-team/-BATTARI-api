using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using BATTARI_api.Models.DTO;
using BATTARI_api.Repository;
using BATTARI_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BATTARI_api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class DeveloperController(IConfiguration configuration, UserOnlineConcurrentDictionaryDatabase userOnlineConcurrentDictionaryDatabase, CallingService callingService) : ControllerBase
{
    /// <summary>
    /// ログインしてないと使えません
    /// </summary>
    /// <returns></returns> <summary>
    ///
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    public IActionResult ConnectionCheck()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claim = identity?.Claims.FirstOrDefault(c => c.Type == "name");

        if (claim != null)
        {
            Console.WriteLine(claim.Value);
        }
        Console.WriteLine(configuration["Pepper"]);

        return Ok("Connection is working. Welcome " + claim.Value + "!");
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    public IActionResult JWTParse(String aiueo)
    {
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(aiueo);
        
        return Ok(jsonToken);
    }
    
    /// <summary>
    ///
    /// </summary>
    [HttpPost]
    public IActionResult TryParseSouguuMaterials(string materials)
    {
            
        Console.WriteLine(materials);
        var souguuMaterials = JsonSerializer.Deserialize<SouguuWebsocketDto>(materials);
        Console.WriteLine("TryParseSouguuMaterials");
        Console.WriteLine(souguuMaterials.incredients.Count);
        Console.WriteLine(souguuMaterials.incredients[0].type);
        return Ok(souguuMaterials);
    }

    [HttpGet]
    public IActionResult ClearUserOnline()
    {
        userOnlineConcurrentDictionaryDatabase.Clear();
        callingService.Clear();
        return Ok("UserOnlineDictionary is cleared");
    }
    
    [HttpGet]
    public IActionResult IsUserSouguu(int userId)
    {
        return Ok(userOnlineConcurrentDictionaryDatabase.IsUserSouguu(userId));
    }
}
