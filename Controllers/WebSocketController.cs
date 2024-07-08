using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class WebSocketController : ControllerBase {
	[Route("/ws")]
	public async Task Get() {
		Console.WriteLine("WebSocket接続開始");
		if (HttpContext.WebSockets.IsWebSocketRequest) {
			// TCP接続をWebSocket接続にアップグレード
			using WebSocket? webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
			await Echo(webSocket);
		}
		else {
			HttpContext.Response.StatusCode = 400;
		}
	}

	[Route("/wstest")]
	public async Task Get2() {
		Console.WriteLine("WebSocket接続開始");
		if (HttpContext.WebSockets.IsWebSocketRequest) {
			// TCP接続をWebSocket接続にアップグレード
			using WebSocket? webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
			await WebSocketTest(webSocket);
		}
		else {
			HttpContext.Response.StatusCode = 400;
		}
	
	}

	private int num = 0;

	private async Task WebSocketTest(WebSocket webSocket) {
		var buffer = new byte[1024 * 4];
		Task task = send(webSocket);
		while(webSocket.State == WebSocketState.Open){

			var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

			if(receiveResult.CloseStatus.HasValue){
				Console.WriteLine("WebSocket接続終了");
				break;
			} 
			if(buffer.Length > 0){
				Console.WriteLine("receive: " + Encoding.Default.GetString(buffer));
			}

		}
		task.Dispose();
		Console.WriteLine("WebSocket接続終了");
	}
	private async Task send(WebSocket webSocket) {
			if(webSocket.State == WebSocketState.Open) {
				await Task.Delay(1000);
				await webSocket.SendAsync(new ArraySegment<byte>(Encoding.Default.GetBytes((num++).ToString())), WebSocketMessageType.Text, true, CancellationToken.None);
				Console.WriteLine("send: " + num);
			}
	}

	private async Task Echo(WebSocket webSocket) {
		Console.WriteLine("Echo");
		var buffer = new byte[1024 * 4];
		var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

		//インターネット接続が切断された時の処理は含まれていないっぽい
		//タイムアウトされたくない時は，クライアント側から定期的にpingを送信する
		while (!receiveResult.CloseStatus.HasValue) {
			await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, receiveResult.Count), receiveResult.MessageType, receiveResult.EndOfMessage, CancellationToken.None);
			receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		}

		await Task.Delay(1000);
		await webSocket.SendAsync(new ArraySegment<byte>(Encoding.Default.GetBytes("hello world"), 0, receiveResult.Count), receiveResult.MessageType, receiveResult.EndOfMessage, CancellationToken.None);
		await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
	}
}
