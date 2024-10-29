using System.Collections.Concurrent;

namespace BATTARI_api.Repository;

public class UserOnlineConcurrentDictionaryDatabase
{
    public ConcurrentDictionary<int, DateTime > UserOnlineDictionary = new ConcurrentDictionary<int, DateTime>();
    
    public void AddUserOnline(int userId)
    {
        UserOnlineDictionary.AddOrUpdate(userId, DateTime.Now, (key, oldValue) => DateTime.Now);
    }
    
    public void RemoveUserOnline(int userId)
    {
        UserOnlineDictionary.TryRemove(userId, out _);
    }
    
    public bool IsUserOnline(int userId)
    {
        if (!UserOnlineDictionary.ContainsKey(userId)) return false;
        
        if(this.UserOnlineDictionary[userId] < DateTime.Now.AddMinutes(-1))
        {
            this.RemoveUserOnline(userId);
            return false;
        }

        return true;
    }
}