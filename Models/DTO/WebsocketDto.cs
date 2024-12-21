using System.Text.Json.Nodes;

namespace BATTARI_api.Models.DTO;

public class WebsocketDto
{
    public string type { get; set; }
    public Object data { get; set; }
}

public class WebsocketDtoForSend
{
    public string type { get; set; }
    public JsonNode? data { get; set; }
}