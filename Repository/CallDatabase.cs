using BATTARI_api.Data;
using BATTARI_api.Migrations;
using BATTARI_api.Models.Data;
using BATTARI_api.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace BATTARI_api.Repository;

public interface ICallRepository
{
    public Task<CallModel> AddCall(string SouguuReason, DateTime callStartTime, int user1, int user2, DateTime? souguuDateTime = null, CallStatusEnum status = CallStatusEnum.Ended);
}
/// <summary>
/// 過去の通話を全て管理するデータベースです
/// </summary>
/// <param name="context"></param>
public class CallDatabase(UserContext context) : ICallRepository
{
    /// <summary>
    /// 通話の登録
    /// </summary>
    /// <param name="SouguuReason"></param>
    /// <param name="callStartTime"></param>
    /// <param name="user1"></param>
    /// <param name="user2"></param>
    /// <param name="souguuDateTime"></param>
    /// <param name="status"></param>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <returns></returns>
    public async Task<CallModel> AddCall(string SouguuReason, DateTime callStartTime, int user1, int user2, DateTime? souguuDateTime = null, CallStatusEnum status = CallStatusEnum.Ended)
    {
        
        var call = new CallModel
        {
            CallStartTime = callStartTime,
            SouguuReason = SouguuReason,
            User1Id = user1,
            User2Id = user2,
            SouguuDateTime = souguuDateTime ?? DateTime.Now,
            Status = status
        };
        var result = context.Calls.Add(call);
        await context.SaveChangesAsync();
        return result.Entity;
    }
}