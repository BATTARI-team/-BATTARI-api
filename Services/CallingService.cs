using System.Collections.Concurrent;
using AgoraIO.Media;
using BATTARI_api.Models.DTO;
using BATTARI_api.Repository;

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
    private readonly IConfiguration _configuration;
    private Task _autoRemover;
    private readonly ILogger<CallingService> _logger;
    private readonly UserOnlineConcurrentDictionaryDatabase _onlineConcurrentDictionaryDatabase;
    
    public CallingService(IConfiguration configuration, ILogger<CallingService> logger, UserOnlineConcurrentDictionaryDatabase onlineConcurrentDictionaryDatabase)
    {
        _userOnlineConcurrentDictionaryDatabase = new ConcurrentDictionary<int, NowCallModel>();
        _onlineConcurrentDictionaryDatabase = onlineConcurrentDictionaryDatabase;
        _configuration = configuration;
        _logger = logger;
        CreateAutoRemover();
    }
    
    private void CreateAutoRemover()
    {
        _autoRemover = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(30000);
                foreach (var user in _userOnlineConcurrentDictionaryDatabase)
                {
                    if (user.Value.IsEnded)
                    {
                        _userOnlineConcurrentDictionaryDatabase.TryRemove(user.Key, out _);
                        _onlineConcurrentDictionaryDatabase.RemoveSouguu(user.Value.User1);
                    }
                }
            }
        });
        _autoRemover.ContinueWith(task =>
                           {
                               CreateAutoRemover();
                           });
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="callId"></param>
    /// <param name="callStartTime"></param>
    /// <param name="callEndTime"></param>
    /// <param name="souguuReason"></param>
    /// <param name="user1"></param>
    /// <param name="user2"></param>
    /// <param name="cancellationReason"></param>
    /// <param name="souguuDateTime"></param>
    /// <returns>配列{channelId, user1Token, user2Token)</returns>
    public string[] AddCall(int callId, DateTime callStartTime, DateTime callEndTime, string souguuReason, int user1, int user2, string cancellationReason, DateTime souguuDateTime)
    {
        Console.WriteLine(callId);
        string callIdStr = callId.ToString();
        try
        {
            string user1Token = _generateToken(user1.ToString(), callIdStr);
            string user2Token = _generateToken(user2.ToString(), callIdStr);
            _userOnlineConcurrentDictionaryDatabase.TryAdd(callId, new NowCallModel(callStartTime, callId, callEndTime, souguuReason, user1, user1Token, user2, user2Token, cancellationReason, souguuDateTime));
        return new[] { callIdStr, user1Token, user2Token };
        }
        catch (Exception e)
        {
            _logger.LogError("トークンの作成に失敗しました", e);
            throw;
        }
    }
    
    public IEnumerable<NowCallModel> GetNowCalls()
    {
        return _userOnlineConcurrentDictionaryDatabase.Values;
    }

    public void Clear()
    {
        _userOnlineConcurrentDictionaryDatabase.Clear();
    }
    private string _generateToken(String uid, string channelId)
    {
        AccessToken accessToken =
            new AccessToken(_configuration["Agora:AppId"] ?? throw new ArgumentNullException("AppIdがappsettings.jsonに設定されていません。,"), _configuration["Agora:AppCertificate"] ?? throw new ArgumentNullException("AppCertificateがappsettings.jsonに設定されていません。"),
                channelId, uid.ToString());
        string result = accessToken.Build();
        _logger.LogInformation("agora token generated successfully, channelId: {channelId}, uid: {uid}", channelId, uid);
        if (result == null)
        {
            throw new Exception("Failed to generate token");
        }
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userIndex"></param>
    /// <returns>現在通話中でなければ，nullを返す</returns>
    public SouguuNotificationDto? GetCall(int userIndex)
    {
        try
        {
        var a = _userOnlineConcurrentDictionaryDatabase.SingleOrDefault<KeyValuePair<int, NowCallModel>>(source => (source.Value.User1 == userIndex || source.Value.User2 == userIndex) && !source.Value.IsEnded);
            SouguuNotificationDto notificationDto = new SouguuNotificationDto()
            {
                CallEndTime = a.Value.CallEndTime,
                CallStartTime = a.Value.CallStartTime,
                CallId = a.Value.CallId,
                SouguuDateTime = a.Value.SouguuDateTime,
                SouguuReason = a.Value.SouguuReason,
                Token = a.Value.User1 == userIndex ? a.Value.User1Token : a.Value.User2Token,
                AiteUserId = a.Value.User1 == userIndex ? a.Value.User2 : a.Value.User1
            };
            return notificationDto;
        }
        catch (Exception)
        {
            return null;
        }
    }
}