using System.ComponentModel.DataAnnotations;

public class FriendModel
{
   [Key]
   public int Id{get;set;}
   public int UserId{get;set;}
   public IEnumerable<int> Frineds{get;set;}
 }
