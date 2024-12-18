using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using BATTARI_api.Models.DTO;
using BATTARI_api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sentry;

namespace BATTARI_api.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class WebSocketController(UserOnlineConcurrentDictionaryDatabase userOnlineConcurrentDictionaryDatabase, ISouguuService souguuService, ILogger<WebSocketContext> logger) : ControllerBase
{
    private async Task KeepAlive(WebSocket webSocket, CancellationToken cancellationToken)
    {
        while (webSocket.State == WebSocketState.Open)
        {
            await Task.Delay(10000, cancellationToken); // 10秒ごとにPingを送信
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(Encoding.UTF8.GetBytes("battari"), WebSocketMessageType.Text, true, cancellationToken);
            }
        }
    }
    
    private void SendNotification(WebSocket websocket, int userId, WebsocketDtoForSend dto, string requestKey)
    {
        if (userOnlineConcurrentDictionaryDatabase.IsUserOnline(userId))
        {
            if(websocket.State == WebSocketState.Open)
            {
                try
                {
                    var json = JsonSerializer.Serialize<WebsocketDtoForSend>(dto, new JsonSerializerOptions{Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping});
                    var bytes = Encoding.UTF8.GetBytes(json);
                    websocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    Console.WriteLine(userId + "に通知を送信しました");
                    SentrySdk.CaptureMessage("Websocket Send notification:" + json + userId + "requestKey:" + requestKey, SentryLevel.Info);
                }
                catch (Exception e)
                {
                    SentrySdk.CaptureException(e);
                    Console.WriteLine(e.ToString());
                }
            }
        }
        else
        {
            SentrySdk.CaptureMessage("Websocket Send notification: user is not online " + userId, SentryLevel.Warning);
        }
    }
    
    /// <summary>
    /// Json
    /// </summary>
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    [Route("/ws")]
    [Authorize]
    public async Task Get()
    {
        var buffer = new byte[1024 * 4];
        bool isEnd = false;
        int userId = Int16.Parse((HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"))!.Value);

        // #TODO ワンチャンbufferを超えたデータを受け取った場合，受け取れきれないかも
        logger.LogInformation($"{userId}:WebSocket接続開始");
        SentrySdk.CaptureMessage($"{userId}:WebSocket接続開始", SentryLevel.Info);
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            
            // TCP接続をWebSocket接続にアップグレード
            using WebSocket webSocket =
                await HttpContext.WebSockets.AcceptWebSocketAsync();
            
            //Task task = send(webSocket);
            Task unused = KeepAlive(webSocket, new CancellationToken());
            var requestKey = HttpContext.TraceIdentifier;
            souguuService.AddSouguuNotification(requestKey, (SouguuNotificationDto dto) =>
            {
                // 遭遇した時に実行したい関数 
                // #TODO variable is disposed in the outer scope
                // obsidian://adv-uri?vault=main&filepath=%2B%2FCsharpdeCaptured%20variable%20is%20disposed%20in%20the%20outer%20scope2024-11-13.md
                // ReSharper disable once AccessToDisposedClosure
                SendNotification(webSocket, userId, new WebsocketDtoForSend(){type = "notification", data = JsonNode.Parse(JsonSerializer.Serialize(dto, new JsonSerializerOptions{Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping}))}, HttpContext.TraceIdentifier);
            }, userId);
            string lastReceived = "";
            while (webSocket.State == WebSocketState.Open)
            {
                using var ms = new MemoryStream();
                String received;
                try
                {
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer),
                            CancellationToken.None);
                        if (result.CloseStatus.HasValue)
                        {
                            logger.LogDebug($"{userId}:WebSocket接続終了");
                            SentrySdk.CaptureMessage($"{userId}:WebSocket接続終了", SentryLevel.Info);
                            isEnd = true;
                            break;
                        }
                        await userOnlineConcurrentDictionaryDatabase.AddUserOnline(userId);

                        ms.Write(buffer, 0, result.Count);
                    } while (!result.EndOfMessage);
                }
                catch (Exception e)
                {
                    logger.LogError("WebSocketの受信に失敗" + e);
                    SentrySdk.CaptureException(e);
                    isEnd = true;
                }
                if(isEnd) break;

                ms.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    received = await reader.ReadToEndAsync();
                }
                if (buffer.Length > 0)
                {
                    logger.LogInformation("Received: {received} from {userId}", received, userId);
                    if(received == "hello")
                    {
                    }
                    else
                    {
                        logger.LogInformation("Received: {received} from {userId}", received, userId);
                        try
                        {
                            lastReceived = received;
                            WebsocketDto? parsed = JsonSerializer.Deserialize<WebsocketDto>(received, _options); if(parsed == null) continue;
                            if (parsed.type.Equals("souguu_materials"))
                            {
                                SouguuWebsocketDto? souguuWebsocketDto =
                                    JsonSerializer.Deserialize<SouguuWebsocketDto>(parsed.data.ToString());
                                if(souguuWebsocketDto == null) continue;
                                if (souguuWebsocketDto.incredients[0] is SouguuAppIncredientModel)
                                {
                                    SouguuAppIncredientModel app = (SouguuAppIncredientModel)souguuWebsocketDto.incredients[0];
                                    Console.WriteLine("app name:" + app.appData.appName);
                                    await souguuService.AddMaterial(souguuWebsocketDto);
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            logger.LogError("jsonのパースに失敗：{}", e.ToString());
                            SentrySdk.CaptureException(e);
                        }    
                    }
                        
                        
                }
            }
            bool isNeedRemove = souguuService.RemoveSouguuNotification(HttpContext.TraceIdentifier);
            logger.LogInformation("websocket 切断:" + webSocket.State);
            SentrySdk.CaptureMessage("websocket 切断:" + webSocket.State, SentryLevel.Info);
            if (isNeedRemove)
                userOnlineConcurrentDictionaryDatabase.RemoveUserOnline(userId);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }
}