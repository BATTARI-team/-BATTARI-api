using BATTARI_api.Models;
using BATTARI_api.Repository.Data;

public class UserViewModel
{
    public IEnumerable<UserModel> Users { get; set; }

    public UserViewModel(UserContext context)
    {
        Users = context.Users.ToList();
    }
}