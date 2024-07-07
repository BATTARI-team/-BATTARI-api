using System.Net.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[AllowAnonymous]
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

	private async Task Echo(WebSocket webSocket) {
		var buffer = new byte[1024 * 4];
		var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

		//インターネット接続が切断された時の処理は含まれていないっぽい
		//タイムアウトされたくない時は，クライアント側から定期的にpingを送信する
		while (!receiveResult.CloseStatus.HasValue) {
			await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, receiveResult.Count), receiveResult.MessageType, receiveResult.EndOfMessage, CancellationToken.None);
			receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		}

		await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
	}
}
