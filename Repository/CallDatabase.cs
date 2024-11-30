using BATTARI_api.Models.Data;
using BATTARI_api.Models.Enum;
using BATTARI_api.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace BATTARI_api.Repository;

public interface ICallRepository
{
    public Task<CallModel> AddCall(string souguuReason, DateTime callStartTime, int user1, int user2, DateTime? souguuDateTime = null, CallStatusEnum status = CallStatusEnum.Ended);
}
/// <summary>
/// 過去の通話を全て管理するデータベースです
/// </summary>
/// <param name="context"></param>
public class CallDatabase(UserContext context, ILogger<CallDatabase> logger) : ICallRepository
{
    /// <summary>
    /// 通話の登録
    /// </summary>
    /// <param name="souguuReason"></param>
    /// <param name="callStartTime"></param>
    /// <param name="user1"></param>
    /// <param name="user2"></param>
    /// <param name="souguuDateTime"></param>
    /// <param name="status"></param>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="DbUpdateConcurrencyException"></exception>
    /// <returns></returns>
    public async Task<CallModel> AddCall(string souguuReason, DateTime callStartTime, int user1, int user2, DateTime? souguuDateTime = null, CallStatusEnum status = CallStatusEnum.Ended)
    {
        Console.WriteLine("通話を追加します");
        var call = new CallModel
        {
            CallStartTime = callStartTime,
            SouguuReason = souguuReason,
            User1Id = user1,
            User2Id = user2,
            SouguuDateTime = souguuDateTime ?? DateTime.Now,
            Status = status,
            // 通話時間を指定する
            CallTime = 1,
        };
        var result = context.Calls.Add(call);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError("通話の追加に失敗しました", e);
        }
        return result.Entity;
    }
}