using System.Collections.Concurrent;
using AgoraIO.Media;

namespace BATTARI_api.Services;

public class NowCallModel
{
    public NowCallModel(DateTime callStartTime, int callId, DateTime callEndTime, string souguuReason, int user1,
        string user1Token, int user2, string user2Token, string cancellationReason, DateTime souguuDateTime)
    {
        this.CallStartTime = callStartTime;
        CallId = callId;
        CallEndTime = callEndTime;
        SouguuReason = souguuReason;
        User1 = user1;
        User1Token = user1Token;
        User2 = user2;
        User2Token = user2Token;
        CancellationReason = cancellationReason;
        SouguuDateTime = souguuDateTime;
    }

    public int CallId { get; }
    public DateTime CallEndTime { get; }
    public DateTime CallStartTime { get; }
    public DateTime SouguuDateTime { get; }
    public string SouguuReason { get; }
    public bool IsEnded => CallEndTime < DateTime.Now;
    public string ChannelId { get; }

public int BufferTimeBeforeCall => (CallEndTime - CallStartTime).Minutes;

    public int User1 { get; }
    public string User1Token { get; }
    public int User2 { get; }
    public string User2Token { get; }
    public string CancellationReason { get; }
}

public class CallingService
{
    private int channelId = 0;
    private readonly ConcurrentDictionary<int, NowCallModel> _userOnlineConcurrentDictionaryDatabase;
    
    public CallingService()
    {
        _userOnlineConcurrentDictionaryDatabase = new ConcurrentDictionary<int, NowCallModel>();
    }
    
    public void AddCall(int callId, DateTime callStartTime, DateTime callEndTime, string souguuReason, int user1, int user2, string cancellationReason, DateTime souguuDateTime)
    {
        string _channelId = (channelId++).ToString();
        string user1Token = "aiueo";
        string user2Token = "aiueo";
        _userOnlineConcurrentDictionaryDatabase.TryAdd(callId, new NowCallModel(callStartTime, callId, callEndTime, souguuReason, user1, user1Token, user2, user2Token, cancellationReason, souguuDateTime));
    }
    
    public IEnumerable<NowCallModel> GetNowCalls()
    {
        return _userOnlineConcurrentDictionaryDatabase.Values;
    }

    public string GetToken(int channelId)
    {
        return "";
    }
}