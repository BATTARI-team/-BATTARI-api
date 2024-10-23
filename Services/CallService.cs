using AgoraIO.Media;

class CallService
{
    ICallRamRepository CallRamDatabase;
    private int index = 0;
    public CallService(ICallRamRepository CallRamDatabase)
    {
        this.CallRamDatabase = CallRamDatabase;
    }

    public async Task<InfoToJoinCallDto> CreateCall(int uid)
    {
        // string channelId = CallRamDatabase.GetChannelId();
        //{
        // token
        // channelId
        // uid
        // }

        string channelId = index++.ToString();
        return new InfoToJoinCallDto(_generateToken(uid.ToString(), channelId),
                                     channelId, uid.ToString());
    }

    private string _generateToken(String uid, string channelId)
    {
        AccessToken accessToken =
            new AccessToken(AgoraVariables.AppId, AgoraVariables.AppCertificate,
                            channelId, uid.ToString());
        string result = accessToken.Build();
        if (result == null)
        {
            throw new Exception("Failed to generate token");
        }
        return result;
    }
}
