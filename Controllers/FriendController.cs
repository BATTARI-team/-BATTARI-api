using BATTARI_api.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
class FriendController
(IUserRepository _userRepository) : Controller
{

    [HttpPost]
    ActionResult PushFriendRequest(int userIndex) { return Ok(); }
}
