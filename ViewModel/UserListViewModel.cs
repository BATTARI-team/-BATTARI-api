using BATTARI_api.Data;
using BATTARI_api.Models;

public class UserViewModel {
    public IEnumerable<UserModel> Users{get;set;}

    public UserViewModel(UserContext context) {
        Users = context.Users.ToList();
    }
}