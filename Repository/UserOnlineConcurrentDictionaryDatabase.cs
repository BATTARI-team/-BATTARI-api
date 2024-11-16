using System.Collections.Concurrent;

namespace BATTARI_api.Repository;

// #TODO サービス化したい
public class UserOnlineConcurrentDictionaryModel()
{
    public DateTime LastOnlineTime { get; set; }
    public bool IsOnline { get { return LastOnlineTime > DateTime.Now.AddSeconds(-30); } }
    /// <summary>
    /// 遭遇しているかを表すフラグ
    /// 遭遇している場合は遭遇相手のuserid, そうでない場合は0
    /// </summary>
    public int IsSouguu { get; set; }
}
public class UserOnlineConcurrentDictionaryDatabase
{
    readonly ConcurrentDictionary<int,  UserOnlineConcurrentDictionaryModel> _userOnlineDictionary = new ConcurrentDictionary<int, UserOnlineConcurrentDictionaryModel>();
    private readonly object _lock = new object();
    private TimeSpan _timeout = TimeSpan.FromSeconds(10);
    private Task _autoRemover;
    private IFriendRepository friendRepository;

    public UserOnlineConcurrentDictionaryDatabase(IFriendRepository friendRepository)
    {
        this.friendRepository = friendRepository;
        CreateAutoRemover();
    }

    private void CreateAutoRemover()
    {
        _autoRemover = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(60000);
                foreach (var user in _userOnlineDictionary)
                {
                    if (!user.Value.IsOnline)
                    {
                        RemoveUserOnline(user.Key);
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
    /// <param name="userId"></param>
    /// <exception cref="TimeoutException"></exception>
    public async Task AddUserOnline(int userId)
    {
        if (Monitor.TryEnter(_lock, _timeout))
        {
            // デッドロックが起きちゃうので，ロックが不要なものはロックしない
            // スレッドセーフのため
            Monitor.Exit(_lock);
            _userOnlineDictionary.AddOrUpdate(userId, new UserOnlineConcurrentDictionaryModel { LastOnlineTime = DateTime.Now }, (key, oldValue) => { oldValue.LastOnlineTime = DateTime.Now; return oldValue; });
        }
    }
    
    public IEnumerable<int> GetOnlineUsers()
    {
        return _userOnlineDictionary.Where((element) => element.Value.IsOnline).Select((element) => element.Key);
    }

    /// <summary>
    /// 指定されたユーザーのフレンドかつオンラインかつ遭遇していないユーザーをリターンします
    /// </summary>
    /// <param name="userIndex"></param>
    /// <returns></returns>
    public async Task<IEnumerable<UserDto>> GetFriendAndOnlineUsers(int userIndex)
    {
        var frind = await friendRepository.GetFriendList(userIndex);
        var friends = (await friendRepository.GetFriendList(userIndex)).Where(
            (element) =>
            {
                // オンラインかつ遭遇してなかったら
                if (IsUserOnline(element.Id) ==
                    (IsUserSouguu(element.Id) == 0))
                {
                    return true;
                }
                return false;
            });
        return friends;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void RemoveUserOnline(int userId)
    {
        if (Monitor.TryEnter(_lock, _timeout))
        {
            Monitor.Exit(_lock);
            _userOnlineDictionary.TryRemove(userId, out _);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool IsUserOnline(int userId)
    {
        if (Monitor.TryEnter(_lock, _timeout))
        {
            Monitor.Exit(_lock);
            if (!_userOnlineDictionary.ContainsKey(userId)) return false;
            
            if(_userOnlineDictionary[userId].IsOnline)
            {
            }
            else
            {
                RemoveUserOnline(userId);
                return false;
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>失敗したらまたは遭遇してなかったら0, 成功したら相手のユーザーid</returns>
    public int IsUserSouguu(int userId)
    {
        if (Monitor.TryEnter(_lock, _timeout))
        {
            Monitor.Exit(_lock);
            if(!_userOnlineDictionary.ContainsKey(userId)) return 0;
            return _userOnlineDictionary[userId].IsSouguu;
        }

        return 0;
    }
    
    /// <summary>
    /// 片方だけやれば両方に適用されるよ
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="souguuUserId"></param>
    public void SetSouguu(int userId, int souguuUserId)
    {
        if(Monitor.TryEnter(_lock, _timeout))
        {
            try
            {
                _userOnlineDictionary[userId].IsSouguu = souguuUserId;
                _userOnlineDictionary[souguuUserId].IsSouguu = userId;
            }finally
            {
                Monitor.Exit(_lock);
            }
        }
    }
    
    public void Clear() => _userOnlineDictionary.Clear();
}