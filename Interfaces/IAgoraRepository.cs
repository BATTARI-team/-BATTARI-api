using AgoraIO.Media;

namespace BATTARI_api.Interfaces;

interface IAgoraRepository
{
    public int GetUid(string channelName);
    public string GetChannelId();
}

class AgoraRepositoryMock : IAgoraRepository
{
    readonly Dictionary<string, int> _uidMap = new Dictionary<string, int>();

    public string GetChannelId()
    {
        return "souguu";
    }

    public int GetUid(string channelId)
    {
        if (_uidMap.ContainsKey(channelId))
            return _uidMap[channelId]++;
        else
            return _uidMap[channelId] = 0;
    }

    public String GetToken(string channelId, int uid)
    {
        return _generateToken(uid, channelId);
    }

    private string _generateToken(int uid, string channelId)
    {
        string appId = "361b3a58935048f2a9e5e5b2a5f70895";
        string appCertificate = "49aa047c914e4af4a7f646d8a3f78f2c";
        AccessToken accessToken = new AccessToken(appId, appCertificate, channelId, uid.ToString());
        var result = accessToken.Build();
        if (result == null) throw new Exception("Token build failed");
        return result;
    }
}