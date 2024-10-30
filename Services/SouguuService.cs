using System.Collections.Concurrent;
using System.Drawing.Printing;
using BATTARI_api.Models;
using BATTARI_api.Models.DTO;
using BATTARI_api.Repository;

public interface ISouguuService
{
    public Task AddIncredient(SouguuWebsocketDto incredients);
}

public class SouguuService : ISouguuService
{
    private readonly UserOnlineConcurrentDictionaryDatabase _userOnlineConcurrentDictionaryDatabase;
    public SouguuService(UserOnlineConcurrentDictionaryDatabase userOnlineConcurrentDictionaryDatabase)
    {
        CreateDequeTask();
        _userOnlineConcurrentDictionaryDatabase = userOnlineConcurrentDictionaryDatabase;
    }
    /// <summary>
    /// 遭遇判定するためのキュー
    /// </summary>
    private ConcurrentQueue<SouguuQueueElement> _souguuQueue = new ConcurrentQueue<SouguuQueueElement>();
    private ConcurrentDictionary<int,SouguuWebsocketDto > _latestIncredient = new ConcurrentDictionary<int, SouguuWebsocketDto>();
    private Task _dequeueTask;

    private class SouguuQueueElement
    {
        public KeyValuePair<int, SouguuWebsocketDto> User1 { get; set; }
        public KeyValuePair<int, SouguuWebsocketDto> User2 { get; set; }
    }
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
        var random = new Random();
        var friends = (await _userOnlineConcurrentDictionaryDatabase.GetFriendAndOnlineUsers(userIndex)).OrderBy(
            x => random.Next());
        // ランダムにfriendsから選ぶ
        foreach (var userDto in friends)
        {
            if(!_latestIncredient.ContainsKey(userDto.Id)) continue;
            
            _souguuQueue.Enqueue(
                new SouguuQueueElement()
                {
                    User1 = new KeyValuePair<int, SouguuWebsocketDto>(userIndex, _latestIncredient[userIndex]),
                    User2 = new KeyValuePair<int, SouguuWebsocketDto>(userIndex, _latestIncredient[userIndex]),
                });
        }
    }
    

    private void notifySouguu(SouguuNotifyElement element)
    {
        
    }

    private void CreateDequeTask()
    {
        // #TODO 結局，キューから取り出したときにもオンラインか判定するんだから，キューにはユーザーインデックスだけ入れればいいと思う
        _dequeueTask = Task.Run(() =>
                {
                    while (true)
                    {
                        Console.WriteLine("dequeue");
                        foreach (var onlineUser in _userOnlineConcurrentDictionaryDatabase.GetOnlineUsers())
                        {
                            Console.WriteLine(onlineUser);
                        }
                        
                        if (_souguuQueue.TryDequeue(out SouguuQueueElement? element))
                        {
                            if (element == null) continue;
                            // ここで遭遇処理を行う
                            Console.WriteLine(element?.User1.Value);
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