using System.Collections.Concurrent;
using Sentry;

namespace BATTARI_api.Repository;

// #TODO サービス化したい
public class UserOnlineConcurrentDictionaryModel
{
    public UserOnlineConcurrentDictionaryModel()
    {
        this.LastSouguuTime = DateTime.MinValue;
    }

    public DateTime LastOnlineTime { get; set; }
    public bool IsOnline => LastOnlineTime > DateTime.Now.AddSeconds(-30);

    /// <summary>
    /// 遭遇しているかを表すフラグ
    /// 遭遇している場合は遭遇相手のuserid, そうでない場合は0
    /// </summary>
    public int IsSouguu { get; set; }
    public DateTime LastSouguuTime { get; set; }
}

public class UserOnlineConcurrentDictionaryDatabase
{
    readonly ConcurrentDictionary<int, UserOnlineConcurrentDictionaryModel> _userOnlineDictionary = new ConcurrentDictionary<int, UserOnlineConcurrentDictionaryModel>();
    private readonly object _lock = new object();
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);
    private Task _autoRemover = null!;
    private readonly IFriendRepository _friendRepository;

    public UserOnlineConcurrentDictionaryDatabase(IFriendRepository friendRepository)
    {
        this._friendRepository = friendRepository;
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
                                                SentrySdk.CaptureMessage("removed online user" + user.Key);
                                                RemoveUserOnline(user.Key);
                                            }
                                        }
                                    }
                                });
        _autoRemover.ContinueWith(
            _ =>
            {
                CreateAutoRemover();
            });
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <exception cref="TimeoutException"></exception>
    public Task AddUserOnline(int userId)
    {
        if (Monitor.TryEnter(_lock, _timeout))
        {
            // デッドロックが起きちゃうので，ロックが不要なものはロックしない
            // スレッドセーフのため
            Monitor.Exit(_lock);
            _userOnlineDictionary.AddOrUpdate(userId, new UserOnlineConcurrentDictionaryModel { LastOnlineTime = DateTime.Now }, (_, oldValue) => { oldValue.LastOnlineTime = DateTime.Now; return oldValue; });
        }

        return Task.CompletedTask;
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
        var frindList = await _friendRepository.GetFriendList(userIndex);
        Console.WriteLine("UserOnlineConcurrentDictionaryDatabase.GetFriendAndOnlineUsers " + frindList.Count());
        var friends = (await _friendRepository.GetFriendList(userIndex)).Where((element) =>
        {
            Console.Write(element.Id + "welcome");
            // オンラインかつ遭遇してなかったら
            if (IsUserOnline(element.Id) ==
                (IsUserSouguu(element.Id) == 0))
            {
                return true;
            }
            SentrySdk.CaptureMessage(
                IsUserOnline(element.Id) == false
                    ? "オンラインじゃない " + element.Id
                    : "遭遇している " + element.Id,
                SentryLevel.Debug);

            return false;
        });
        return friends;
    }

    public void SetLastSouguu(int userId)
    {
        if (Monitor.TryEnter(_lock, _timeout))
        {
            Monitor.Exit(_lock);
            _userOnlineDictionary[userId].LastSouguuTime = DateTime.Now;
        }
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
            if (_userOnlineDictionary[userId].IsSouguu == 0)
            {
                _userOnlineDictionary.TryRemove(userId, out _);
                _userOnlineDictionary.Remove(userId, out _);
            }
            else
            {
                Console.WriteLine("遭遇しているユーザーを削除しようとしました");
            }
        }
        else
        {
            throw new ArgumentNullException("削除できませんでした");
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
            if (!_userOnlineDictionary.ContainsKey(userId))
                return false;

            if (_userOnlineDictionary[userId].IsOnline)
            {
            }
            else
            {
                RemoveUserOnline(userId);
                return false;
            }
            return true;
        }
        else
        {
            throw new ArgumentNullException("削除できませんでした");
        }

        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>失敗したら-1, 遭遇してなかったら0, 成功したら相手のユーザーid</returns>
    public int IsUserSouguu(int userId)
    {
        while (Monitor.TryEnter(_lock, _timeout))
        {
            Monitor.Exit(_lock);
            if (!_userOnlineDictionary.ContainsKey(userId))
                return 0;
            return _userOnlineDictionary[userId].IsSouguu;
        }
        return -1;
    }

    /// <summary>
    /// 片方だけやれば両方に適用されるよ
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="souguuUserId"></param>
    public void SetSouguu(int userId, int souguuUserId)
    {
        if (Monitor.TryEnter(_lock, _timeout))
        {
            try
            {
                _userOnlineDictionary[userId].IsSouguu = souguuUserId;
                _userOnlineDictionary[souguuUserId].IsSouguu = userId;
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }
    }

    public void RemoveSouguu(int userId)
    {
        if (Monitor.TryEnter(_lock, _timeout))
        {
            try
            {
                _userOnlineDictionary[_userOnlineDictionary[userId].IsSouguu].IsSouguu = 0;
                _userOnlineDictionary[userId].IsSouguu = 0;

                SentrySdk.CaptureMessage("souguu removed" + _userOnlineDictionary[userId].IsSouguu + " " + _userOnlineDictionary[_userOnlineDictionary[userId].IsSouguu].IsSouguu);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                Console.WriteLine(e);
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }
        else
        {
            throw new ArgumentNullException("削除できませんでした");
        }
    }

    public void Clear() => _userOnlineDictionary.Clear();

    public UserOnlineConcurrentDictionaryModel this[int key]
    {
        get => _userOnlineDictionary[key];
    }
}
