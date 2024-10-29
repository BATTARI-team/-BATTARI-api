using System.Collections.Concurrent;

namespace BATTARI_api.Repository;

public class UserOnlineConcurrentDictionaryModel
{

    public DateTime LastOnlineTime { get; set; }
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
            
            if(_userOnlineDictionary[userId].LastOnlineTime < DateTime.Now.AddMinutes(-1))
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
    /// <returns>失敗したら0, 成功したら相手のユーザーid</returns>
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
    /// 
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
}