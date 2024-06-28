using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BATTARI_api.Models;

public class UserModel {
    [MaxLength(30)]
    public required string Name { get; set; }
    public Gps? Gps { get; set; }
    public int Id { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }
    public required DateTime Created { get; set; }
}