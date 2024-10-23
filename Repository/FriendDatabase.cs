using BATTARI_api.Data;
using Microsoft.EntityFrameworkCore;

namespace BATTARI_api.Repository
{
    interface IFriendRepository
    {
        Task<FriendModel?> IsExist(int user1, int user2);
    }

    public class FriendDatabase : IFriendRepository
    {
        private readonly UserContext _context;
        public FriendDatabase(UserContext context) { _context = context; }
        public async Task<FriendModel?> IsExist(int user1, int user2)
        {
            FriendModel? result = await _context.Friends.FirstOrDefaultAsync(
                x => (x.User1Id == user1 && x.User2Id == user2) ||
                     (x.User1Id == user2 && x.User2Id == user1));
            if (result == null)
                return null;
            else
                return result;
        }
    }
}
