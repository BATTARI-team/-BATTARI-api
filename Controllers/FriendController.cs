using System.Security.Claims;
using BATTARI_api.Interfaces;
using BATTARI_api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("[controller]/[action]")]
public class FriendController
(IUserRepository _userRepository, IFriendRepository _friendRepository)
    : Controller
{

    /// <summary>
    /// 引数のユーザー宛に友達申請を送信します．
    /// もし相手から友達申請がある場合は，友達登録を行います（友達として成立）
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<FriendRequestDto>> PushFriendRequest(
        int userIndex)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claim = HttpContext.User.Claims.FirstOrDefault(
            c => c.Type ==
                 "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        if (claim == null)
            return BadRequest();
        int claimId;
        try
        {
            claimId = int.Parse(claim.Value);
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
        if (claimId == userIndex)
            return BadRequest("自分自身に友達申請はできません");
        try
        {
            FriendStatusEnum? friendStatusEnum =
                await _friendRepository.AddFriendRequest(claimId, userIndex);
            if (friendStatusEnum == null)
                return BadRequest();
            else
            {
                return new FriendRequestDto()
                {
                    User1 = claimId,
                    User2 = userIndex,
                    Status = (FriendStatusEnum)friendStatusEnum
                };
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    ///     /// <summary>
    /// ユーザーの友達一覧を出力します
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetFriends()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claim = HttpContext.User.Claims.FirstOrDefault(
            c => c.Type ==
                 "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        if (claim == null)
            return BadRequest();
        int claimId;
        try
        {
            claimId = int.Parse(claim.Value);
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
        try
        {
            IEnumerable<UserDto> friendList =
                await _friendRepository.GetFriendList(claimId);
            return Ok(friendList);
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    /// <summary>
    /// 入力されたユーザーとの友達関係を出力します
    /// 0: リクエスト中
    /// 1: 友達
    /// 2: なし
    /// </summary>
    /// <returns></returns>
    /// <arg name="claimId">相手のユーザーID</arg>
    [HttpGet]
    public async Task<ActionResult<FriendRequestDto>> GetFriendStatus(
        int userIndex)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claim = HttpContext.User.Claims.FirstOrDefault(
            c => c.Type ==
                 "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        if (claim == null)
            return BadRequest("tokenが無効です");
        int claimId;
        try
        {
            claimId = int.Parse(claim.Value);
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
        try
        {
            FriendModel? friendModel =
                await _friendRepository.IsExist(claimId, userIndex);
            if (friendModel == null)
                return new FriendRequestDto()
                {
                    User1 = claimId,
                    User2 = userIndex,
                    Status = FriendStatusEnum.none
                };
            else
            {
                return new FriendRequestDto()
                {
                    User1 = claimId,
                    User2 = userIndex,
                    Status = friendModel.Status
                };
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    /// <summary>
    /// ユーザーに届いている友達申請一覧を出力します
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<FriendRequestDto>>> GetFriendRequests()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claim = HttpContext.User.Claims.FirstOrDefault(
            c => c.Type ==
                 "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        if (claim == null)
            return BadRequest();
        int claimId;
        try
        {
            claimId = int.Parse(claim.Value);
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
        try
        {
            IEnumerable<int> friendRequestList =
                await _friendRepository.GetFriendRequests(claimId);
            return Ok(friendRequestList);
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }
}
