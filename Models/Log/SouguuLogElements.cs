using BATTARI_api.Models.DTO;

namespace BATTARI_api.Models.Log;

public class CheckSouguuLogElement(
    string message,
    int userId1,
    SouguuWebsocketDto souguuWebsocketDtoUser1,
    int userId2,
    SouguuWebsocketDto souguuWebsocketDtoUser2,
    string result)
{
    public string Message { get; set; } = message;
    public int UserId1 { get; set; } = userId1;
    public SouguuWebsocketDto SouguuWebsocketDtoUser1 { get; set; } = souguuWebsocketDtoUser1;
    public int UserId2 { get; set; } = userId2;
    public SouguuWebsocketDto SouguuWebsocketDtoUser2 { get; set; } = souguuWebsocketDtoUser2;
    public String Result { get; set; } = result;
}