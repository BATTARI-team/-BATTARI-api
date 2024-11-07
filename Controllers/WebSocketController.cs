using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BATTARI_api.Models.DTO;
using BATTARI_api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiExplorerSettings(IgnoreApi = true)]
public class WebSocketController(UserOnlineConcurrentDictionaryDatabase userOnlineConcurrentDictionaryDatabase, ISouguuService souguuService, ILogger<WebSocketContext> _logger) : ControllerBase
{
    private async Task KeepAlive(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            await Task.Delay(10000, cancellationToken); // 10秒ごとにPingを送信
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(UnicodeEncoding.UTF8.GetBytes("battari"), WebSocketMessageType.Text, true, cancellationToken);
            }
        }
    }
    
    /// <summary>
    /// Json
    /// </summary>
    private JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    [Route("/ws")]
    [Authorize]
    public async Task Get()
    {
        var buffer = new byte[1024 * 4];
        bool isEnd = false;
        var end = isEnd;
        int userId = Int16.Parse((HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"))!.Value);

        async Task close(int userId)
        {
            userOnlineConcurrentDictionaryDatabase.RemoveUserOnline(userId);
        }

        async Task send(WebSocket webSocket)
        {
            while (webSocket.State == WebSocketState.Open)
            {
                if (end) break;
                await webSocket.SendAsync(
                    new ArraySegment<byte>(
                        Encoding.Default.GetBytes("battari")),
                    WebSocketMessageType.Text, true, CancellationToken.None);
                await Task.Delay(10000);
            }
        }

        // #TODO ワンチャンbufferを超えたデータを受け取った場合，受け取れきれないかも
        _logger.LogInformation($"{userId}:WebSocket接続開始");
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            
            // TCP接続をWebSocket接続にアップグレード
            using WebSocket? webSocket =
                await HttpContext.WebSockets.AcceptWebSocketAsync();
            
            //Task task = send(webSocket);
            Task task2 = KeepAlive(webSocket, new CancellationToken());
            while (webSocket.State == WebSocketState.Open)
            {
                using (var ms = new MemoryStream())
                {
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
                                _logger.LogDebug($"{userId}:WebSocket接続終了");
                                isEnd = true;
                                break;
                            }
                            await userOnlineConcurrentDictionaryDatabase.AddUserOnline(userId);

                            ms.Write(buffer, 0, result.Count);
                        } while (!result.EndOfMessage);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("WebSocketの受信に失敗" + e);
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
                        _logger.LogInformation("Received: {received} from {userId}", received, userId);
                        if(received == "hello")
                        {
                        }
                        else
                        {
                            try
                            {
                                SouguuWebsocketDto? parsed = JsonSerializer.Deserialize<SouguuWebsocketDto>(received, options);
                                if(parsed == null) continue;
                                if (parsed?.incredients[0] is SouguuAppIncredientModel)
                                {
                                    SouguuAppIncredientModel app = (SouguuAppIncredientModel)parsed.incredients[0];
                                    Console.WriteLine("appname:" + app.appData.appName);
                                }

                                await souguuService.AddMaterial(parsed);


                            }
                            catch (Exception e)
                            {
                                _logger.LogError("jsonのパースに失敗：e.ToString()");
                            }    
                        }
                        
                        
                    }

                }
                
            }
            Console.WriteLine("切断されたようです" + webSocket.State);
            _logger.LogInformation("websocekt切断:" + webSocket.State);
            close(userId);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }
}
