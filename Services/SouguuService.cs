using System.Collections.Concurrent;
using System.Drawing.Printing;
using System.Text.Json;
using BATTARI_api.Models;
using BATTARI_api.Models.Data;
using BATTARI_api.Models.DTO;
using BATTARI_api.Models.Enum;
using BATTARI_api.Models.Log;
using BATTARI_api.Repository;
using BATTARI_api.Services;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

public interface ISouguuService
{
    public Task AddMaterial(SouguuWebsocketDto materials);
    public void AddSouguuNotification(string requestId, Action<SouguuNotificationDto> action, int userIndex);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns>もし他にこのユーザーへのwebsocket接続が存在しなかった場合はtrueを返す．つまりオンラインユーザーから省くべき場合はtrue</returns>
    public bool RemoveSouguuNotification(string requestId);
}

public class SouguuService : ISouguuService
{
    private readonly UserOnlineConcurrentDictionaryDatabase _userOnlineConcurrentDictionaryDatabase;
    private readonly ICallRepository _callRepository;
    private readonly CallingService _callingService;
    private readonly ILogger<ISouguuService> _logger;
    public SouguuService(UserOnlineConcurrentDictionaryDatabase userOnlineConcurrentDictionaryDatabase, CallingService callingService, ILogger<ISouguuService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        CreateDequeTask();
        _userOnlineConcurrentDictionaryDatabase = userOnlineConcurrentDictionaryDatabase;
        _callingService = callingService;
        _callRepository = serviceScopeFactory.CreateScope( ).ServiceProvider.GetRequiredService<ICallRepository>();
        _logger = logger;
        Console.WriteLine("SouguuServiceが作成されました");
    }
    /// <summary>
    /// 遭遇判定するためのキュー
    /// </summary>
    private readonly ConcurrentQueue<int> _souguuQueue = new ConcurrentQueue<int>();
    public ConcurrentDictionary<int,SouguuWebsocketDto > _latestIncredient = new ConcurrentDictionary<int, SouguuWebsocketDto>();
    private ConcurrentDictionary<string, Action<SouguuNotificationDto>> _souguuNotification = new ConcurrentDictionary<string, Action<SouguuNotificationDto>>();
    private ConcurrentDictionary<string, int> _requestIdToUserIndex = new ConcurrentDictionary<string, int>();
    private Task _dequeueTask;
    
    public void AddSouguuNotification(string requestId, Action<SouguuNotificationDto> action, int userIndex)
    {
        _souguuNotification.AddOrUpdate(requestId, i => action, (i, action1) => action);
        _requestIdToUserIndex.AddOrUpdate(requestId, userIndex, (i, index) => userIndex);
    }
    
    public bool RemoveSouguuNotification(string requestId)
    {
        _souguuNotification.TryRemove(requestId, out _);
        var userIndex = _requestIdToUserIndex[requestId];
        _requestIdToUserIndex.TryRemove(requestId, out _);
        return _requestIdToUserIndex.Any(
             i => i.Value == userIndex);
    }
    
    /// <summary>
    /// 遭遇の判断材料を入れる
    /// </summary>
    /// <param name="materials"></param>
    /// <param name="userIndex"></param>
    public async Task AddMaterial(SouguuWebsocketDto materials)
    {
        Console.WriteLine(_latestIncredient.Count);
        _latestIncredient[materials.id] = materials;
        Console.WriteLine("追加されました　from " + materials.id);
        await AddSouguuQueueElement(materials.id);
        Console.WriteLine(_latestIncredient.Count);
    }

    private async Task AddSouguuQueueElement(int userIndex)
    {
        _logger.LogDebug(userIndex + " enqueue");
        _souguuQueue.Enqueue(userIndex);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user1"></param>
    /// <param name="user2"></param>
    /// <param name="reason"></param>
    private async Task Souguu(int user1, int user2, SouguuReasonStatusEnum reason)
    {
        Console.WriteLine("遭遇しました");
        CallModel call;
        
        // 配列{channelId, user1Token, user2Token)
        string[] callDetail;
        try
        {
            call = await _callRepository.AddCall(souguuReason: "battari", callStartTime: DateTime.Now.AddSeconds(15),
                user1: user1,
                user2: user2, souguuDateTime: DateTime.Now, status: CallStatusEnum.Waiting);
            callDetail = _callingService.AddCall(
                callId: call.CallId,
                callStartTime: call.CallStartTime,
                callEndTime: call.CallStartTime.AddMinutes(call.CallTime),
                souguuReason: call.SouguuReason,
                user1: call.User1Id,
                user2: call.User2Id,
                cancellationReason: "",
                souguuDateTime: call.SouguuDateTime
            );
        }
        catch (DbUpdateException e)
        {
            Console.WriteLine("データベースに保存できませんでした" + e);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError("遭遇処理に失敗しました", e);
            throw;
        }
        
        _userOnlineConcurrentDictionaryDatabase.SetSouguu(user1, user2);
        var user1RequestIds = _requestIdToUserIndex.Where(i => i.Value == user1).Select(i => i.Key).ToList();
        var user2RequestIds = _requestIdToUserIndex.Where(i => i.Value == user2).Select(i => i.Key).ToList();
        foreach (var user1RequestId in user1RequestIds)
        {
            _souguuNotification.TryGetValue(user1RequestId, out var user1Notification);
            if (user1Notification != null)
            {
                user1Notification(new SouguuNotificationDto()
                {
                    CallEndTime = call.CallStartTime.AddMinutes(call.CallTime),
                    CallStartTime = call.CallStartTime,
                    CallId = call.CallId,
                    SouguuDateTime = call.SouguuDateTime,
                    SouguuReason = call.SouguuReason,
                    Token = callDetail[1],
                    AiteUserId = user2
                });
            }
        }
        
        foreach (var user2RequestId in user2RequestIds)
        {
            _souguuNotification.TryGetValue(user2RequestId, out var user2Notification);
            if (user2Notification != null)
            {
                user2Notification(new SouguuNotificationDto()
                {
                    CallEndTime = call.CallStartTime.AddMinutes(call.CallTime),
                    CallStartTime = call.CallStartTime,
                    CallId = call.CallId,
                    SouguuDateTime = call.SouguuDateTime,
                    SouguuReason = call.SouguuReason,
                    Token = callDetail[2],
                    AiteUserId = user1
                });
            }
        }

        _logger.LogInformation("{}と{}が遭遇しました⭐⭐️⭐️️", user1, user2);
    }

    // #TODO 遭遇材料が古かったら，どうしよう
    private async Task SouguuCheck(int user1, int user2)
    {
        _logger.LogInformation("遭遇判定: {user1}と{user2}", user1, user2);
        if (!_latestIncredient.ContainsKey(user2)) return;
        Console.WriteLine("ここまで[{s");
        
        var user1Materials = _latestIncredient[user1];
        var user2Materials = _latestIncredient[user2];
        SouguuReasonStatusEnum? result = null;
        
        if(user1Materials.isWelcome && user2Materials.isWelcome)
        {
            // ここで遭遇処理を行う
            await Souguu(user1, user2, SouguuReasonStatusEnum.Battari_Welcome);
            result = SouguuReasonStatusEnum.Battari_Welcome;
        }

        SouguuAppIncredientModel? user1AppUsage = null;
        foreach (var VARIABLE in user1Materials.incredients)
        {
            if (VARIABLE is not SouguuAppIncredientModel model) continue;
            user1AppUsage = model;
            break;
        }
        Console.Write("user1AppUsage");
        Console.WriteLine(user1AppUsage);

        SouguuAppIncredientModel? user2AppUsage = null;
        foreach (var VARIABLE in user2Materials.incredients)
        {
            if (VARIABLE is not SouguuAppIncredientModel model) continue;
            user2AppUsage = model;
            break;
        }
        if(user1AppUsage == null || user2AppUsage == null) return;
        _logger.LogInformation("ここまで");
        Console.WriteLine("user1AppUsage" + user1AppUsage.appData.appName + "\nuser2AppUsage" + user2AppUsage.appData.appName);
        if (String.Compare(user1AppUsage.appData.appName, user2AppUsage.appData.appName, StringComparison.Ordinal) == 0)
        {
            Console.WriteLine("同じアプリを使っている");
            await Souguu(user1, user2, SouguuReasonStatusEnum.App_Usage);
            result = SouguuReasonStatusEnum.App_Usage;
        }
        _logger.LogInformation("ここまで");

        string resultString = "";
        if(result == null) resultString = "";
        else resultString = result.ToString();
        var logmodel = (new CheckSouguuLogElement("遭遇判定", user1, user1Materials, user2, user2Materials, resultString))
            .ToString();
        _logger.LogInformation("遭遇判定終了: {user1}と{user2} {result}", user1, user2, result);
        _logger.LogInformation(JsonSerializer.Serialize(logmodel));
    }
    
    private void CreateDequeTask()
    {
        Random random = new Random();
        _dequeueTask = Task.Run(async () =>
                {
                    while (true)
                    {
                        foreach (var VARIABLE in _callingService.GetNowCalls()) 
                        {
                            Console.WriteLine("call: " + VARIABLE.CallId + " " + VARIABLE.User1 + " " + VARIABLE.User2);
                        }

                        if (_souguuQueue.TryDequeue(out int element))
                        {
                            if (element == 0) continue;
                            // ここで遭遇処理を行う
                            var users = await _userOnlineConcurrentDictionaryDatabase.GetFriendAndOnlineUsers(element);
                            var friends = (await _userOnlineConcurrentDictionaryDatabase.GetFriendAndOnlineUsers(element)).OrderBy(
                                (_) => random.Next());
                            if (!friends.Any()) continue;
                            foreach (var VARIABLE in friends)
                            {
                                Console.WriteLine(VARIABLE.Name);
                                await SouguuCheck(element, VARIABLE.Id);
                            }
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                    }
                });
                _dequeueTask.ContinueWith(task =>
                {
                    CreateDequeTask();
                });
    }
}