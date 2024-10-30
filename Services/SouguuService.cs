using System.Collections.Concurrent;
using System.Drawing.Printing;
using BATTARI_api.Models;
using BATTARI_api.Models.DTO;
using BATTARI_api.Models.Enum;
using BATTARI_api.Repository;
using BATTARI_api.Services;

public interface ISouguuService
{
    public Task AddIncredient(SouguuWebsocketDto incredients);
}

public class SouguuService : ISouguuService
{
    private readonly UserOnlineConcurrentDictionaryDatabase _userOnlineConcurrentDictionaryDatabase;
    private readonly ICallRepository _callRepository;
    private readonly CallingService _callingService;
    public SouguuService(UserOnlineConcurrentDictionaryDatabase userOnlineConcurrentDictionaryDatabase, CallingService callingService)
    {
        CreateDequeTask();
        _userOnlineConcurrentDictionaryDatabase = userOnlineConcurrentDictionaryDatabase;
        _callingService = callingService;
    }
    /// <summary>
    /// 遭遇判定するためのキュー
    /// </summary>
    private ConcurrentQueue<int> _souguuQueue = new ConcurrentQueue<int>();
    private ConcurrentDictionary<int,SouguuWebsocketDto > _latestIncredient = new ConcurrentDictionary<int, SouguuWebsocketDto>();
    private Task _dequeueTask;

    /// <summary>
    /// 遭遇したユーザー一人ずつに作成される
    /// </summary>
    private class SouguuNotifyElement
    {
        int User1 { get; set; }
        int User2 { get; set; }
        string Reason { get; set; }
        DateTime callStartTime { get; set; }
    }
    
    /// <summary>
    /// 遭遇の判断材料を入れる
    /// </summary>
    /// <param name="incredients"></param>
    /// <param name="userIndex"></param>
    public async Task AddIncredient(SouguuWebsocketDto incredients)
    {
        _latestIncredient.AddOrUpdate(incredients.id, i => incredients, (i, model) => incredients);
        await AddSouguuQueueElement(incredients.id);
        Console.WriteLine("added incredient");
    }

    private async Task AddSouguuQueueElement(int userIndex)
    {
        _souguuQueue.Enqueue(userIndex);
    }
    

    private void notifySouguu(SouguuNotifyElement element)
    {
        
    }

    private async Task Souguu(int user1, int user2, SouguuReasonStatusEnum reason)
    {
        // エラーハンドリングをしっかり
        var call = await _callRepository.AddCall(SouguuReason: "battari", callStartTime: DateTime.Now.AddMinutes(2), user1: user1,
            user2: user2, souguuDateTime: DateTime.Now, status: CallStatusEnum.Waiting);
        _callingService.AddCall(
            callId:call.CallId,
            callStartTime: call.CallStartTime,
            callEndTime: call.CallStartTime.AddMinutes(call.CallTime),
            souguuReason: call.SouguuReason,
            user1: call.User1Id,
            user2: call.User2Id,
            user1Token:"",
            user2Token:"",
            cancellationReason:"",
            souguuDateTime: call.SouguuDateTime
        );
    }

    private async Task SouguuCheck(int user1, int user2)
    {
        if (!_latestIncredient.ContainsKey(user2)) return;
        
        var user1Incredients = _latestIncredient[user1];
        var user2Incredients = _latestIncredient[user2];
        
        if(user1Incredients.isWelcome && user2Incredients.isWelcome)
        {
            // ここで遭遇処理を行う
        }
    }
    
    private void CreateDequeTask()
    {
        Random random = new Random();
        // #TODO 結局，キューから取り出したときにもオンラインか判定するんだから，キューにはユーザーインデックスだけ入れればいいと思う
        _dequeueTask = Task.Run(async () =>
                {
                    while (true)
                    {
                        Console.WriteLine("dequeue");
                        foreach (var onlineUser in _userOnlineConcurrentDictionaryDatabase.GetOnlineUsers())
                        {
                            Console.WriteLine(onlineUser);
                        }
                        
                        if (_souguuQueue.TryDequeue(out int element))
                        {
                            if (element == 0) continue;
                            // ここで遭遇処理を行う
                            var friends = (await _userOnlineConcurrentDictionaryDatabase.GetFriendAndOnlineUsers(element)).OrderBy(
                                (_) => random.Next());
                            if (!friends.Any()) continue;
                            foreach (var VARIABLE in friends)
                            {
                                await SouguuCheck(element, VARIABLE.Id);
                            }
                            
                            Console.WriteLine("check");
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