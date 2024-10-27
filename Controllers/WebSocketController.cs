using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BATTARI_api.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiExplorerSettings(IgnoreApi = true)]
public class WebSocketController : ControllerBase
{
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

        async Task send(WebSocket webSocket)
        {
            if (webSocket.State == WebSocketState.Open)
            {
                await Task.Delay(1000);
                await webSocket.SendAsync(
                    new ArraySegment<byte>(Encoding.Default.GetBytes((num++).ToString())),
                    WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine("send: " + num);
                while (webSocket.State == WebSocketState.Open)
                {
                    if (end) break;
                    await Task.Delay(1000);
                    await webSocket.SendAsync(
                        new ArraySegment<byte>(
                            Encoding.Default.GetBytes((num++).ToString())),
                        WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        // #TODO ワンチャンbufferを超えたデータを受け取った場合，受け取れきれないかも
        Console.WriteLine("WebSocket接続開始");
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            
            // TCP接続をWebSocket接続にアップグレード
            using WebSocket? webSocket =
                await HttpContext.WebSockets.AcceptWebSocketAsync();
            
            Task echoTask =  send(webSocket);
            while (webSocket.State == WebSocketState.Open)
            {
                await webSocket.SendAsync(
                    new ArraySegment<byte>(Encoding.Default.GetBytes("hello")),
                    WebSocketMessageType.Text, true, CancellationToken.None);
                using (var ms = new MemoryStream())
                {
                    String received;
                    WebSocketReceiveResult result;
                    try
                    {
                        do
                        {
                            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer),
                                CancellationToken.None);
                            if (result.CloseStatus.HasValue)
                            {
                                Console.WriteLine("WebSocket接続終了");
                                isEnd = true;
                                break;
                            }

                            ms.Write(buffer, 0, result.Count);
                        } while (!result.EndOfMessage);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    if(isEnd) break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        received = await reader.ReadToEndAsync();
                    }
                    if (buffer.Length > 0)
                    {
                        Console.WriteLine("receive: " + received);
                        try
                        {
                            var parsed = JsonSerializer.Deserialize<SouguuWebsocketDto>(received, options);
                            if (parsed != null)
                            {
                                if (parsed.incredients[0] is SouguuAppIncredientModel)
                                {
                                    SouguuAppIncredientModel app = (SouguuAppIncredientModel)parsed.incredients[0];
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        
                    }

                }
                
            }
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }

    //[Route("/wstest")]
    //[Authorize]
    //public async Task Get2(String token)
    //{
    //    Console.WriteLine(token);
    //    Console.WriteLine("WebSocket接続開始");
    //    // var identity = HttpContext.User.Identity as ClaimsIdentity;

    //    if (HttpContext.WebSockets.IsWebSocketRequest)
    //    {
    //        Console.WriteLine(HttpContext.Request.Headers["Authorization"]);
    //        var identity = HttpContext.User.Identity as ClaimsIdentity;
    //        var claim = identity?.Claims.FirstOrDefault(c => c.Type == "name");

    //        if (claim != null)
    //        {
    //            Console.WriteLine(claim.Value);
    //        }

    //        // TCP接続をWebSocket接続にアップグレード
    //        using WebSocket? webSocket =
    //            await HttpContext.WebSockets.AcceptWebSocketAsync();
    //        await WebSocketTest(webSocket);

    //    }
    //    else
    //    {
    //        HttpContext.Response.StatusCode = 400;
    //    }
    //}

    [Route("/souguu")]
    public async Task Souguu()
    {
        Console.WriteLine("init souguu websocket");
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            // TCP接続をWebSocket接続にアップグレード
            using WebSocket? webSocket =
                await HttpContext.WebSockets.AcceptWebSocketAsync();
            await SouguuWebSocket(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }

    public async Task SouguuWebSocket(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        Task task = SouguuSender(webSocket);
        while (webSocket.State == WebSocketState.Open)
        {
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
            if (receiveResult.CloseStatus.HasValue)
            {
                Console.WriteLine("WebSocket接続終了");
                break;
            }
        }
        task.Dispose();
    }
    ///
    /// 遭遇せんだー
    ///
    private async Task SouguuSender(WebSocket webSocket)
    {
        while (webSocket.State == WebSocketState.Open)
        {
            await Task.Delay(100);
            if (isSouguu)
            {
                isSouguu = false;
                await webSocket.SendAsync(
                    new ArraySegment<byte>(Encoding.Default.GetBytes("true")),
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    public static bool isSouguu = false;

    private int num = 0;

    //private async Task WebSocketTest(WebSocket webSocket)
    //{
    //    var buffer = new byte[1024 * 4];
    //    // データの送信
    //    Task task = send(webSocket);
    //    while (webSocket.State == WebSocketState.Open)
    //    {

    //        // データの受信
    //        var receiveResult = await webSocket.ReceiveAsync(
    //            new ArraySegment<byte>(buffer), CancellationToken.None);

    //        if (receiveResult.CloseStatus.HasValue)
    //        {
    //            Console.WriteLine("WebSocket接続終了");
    //            break;
    //        }
    //        if (buffer.Length > 0)
    //        {
    //            Console.WriteLine("receive: " + Encoding.Default.GetString(buffer));
    //        }
    //    }
    //    task.Dispose();
    //    Console.WriteLine("WebSocket接続終了");
    //}

    private async Task Echo(WebSocket webSocket)
    {
        Console.WriteLine("Echo");
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        // インターネット接続が切断された時の処理は含まれていないっぽい
        // タイムアウトされたくない時は，クライアント側から定期的にpingを送信する
        while (!receiveResult.CloseStatus.HasValue)
        {
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType, receiveResult.EndOfMessage,
                CancellationToken.None);
            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await Task.Delay(1000);
        await webSocket.SendAsync(
            new ArraySegment<byte>(Encoding.Default.GetBytes("hello world"), 0,
                                   receiveResult.Count),
            receiveResult.MessageType, receiveResult.EndOfMessage,
            CancellationToken.None);
        //await webSocket.CloseAsync(receiveResult.CloseStatus.Value,
        //                           receiveResult.CloseStatusDescription,
        //                           CancellationToken.None);
    }
}
