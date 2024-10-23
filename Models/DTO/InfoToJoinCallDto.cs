class InfoToJoinCallDto
{
    public string token { get; }
    public string channelId { get; }
    public string uid { get; }

    public InfoToJoinCallDto(string token, string channelId, string uid)
    {
        this.token = token;
        this.channelId = channelId;
        this.uid = uid;
    }
}
