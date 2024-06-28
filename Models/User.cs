namespace BATTARI_api.Models;

class UserModel {
    public required string Name { get; set; }
    public Gps? Gps { get; set; }
    public required int Id { get; set; }
}