// using System;
// using Java.Lang;
// using Square.OkHttp3;
// using Square.OkIO;
// using WebRTC.H113.Signaling.WebSocket;
// using Exception = System.Exception;
//
// namespace WebRTC.H113.Droid
// {
//     public class OkHttpWebSocket : IWebSocketConnection
//     {
//         private readonly WebSocketListenerEx _listener;
//         private IWebSocket _webSocket;
//
//         public OkHttpWebSocket()
//         {
//             _listener = new WebSocketListenerEx(this);
//         }
//
//         public event EventHandler OnOpened;
//         public event EventHandler<(int code, string reason)> OnClosed;
//         public event EventHandler<Exception> OnError;
//         public event EventHandler<string> OnMessage;
//
//         public void Dispose()
//         {
//             _webSocket.Dispose();
//             _listener.Dispose();
//         }
//
//         public void Open(string url, string protocol = null, string authToken = null)
//         {
//             var requestBuilder = new Request.Builder()
//                 .Url(url);
//             if (!string.IsNullOrEmpty(protocol))
//             {
//                 requestBuilder.AddHeader("Sec-WebSocket-Protocol", protocol);
//             }
//
//             requestBuilder.Header("Origin", url);
//
//             var request = requestBuilder.Build();
//
//             var client = new OkHttpClient();
//
//             
//             _webSocket = client.NewWebSocket(request, _listener);
//         }
//
//         public void Close()
//         {
//             _webSocket.Close(1000, null);
//         }
//
//         public void Send(string message)
//         {
//             _webSocket.Send(message);
//         }
//
//         private void SendOnOpened()
//         {
//             OnOpened?.Invoke(this, EventArgs.Empty);
//         }
//
//         private void SendOnClose(int code, string reason)
//         {
//             OnClosed?.Invoke(this, (code, reason));
//         }
//
//         private void SendOnError(Exception error)
//         {
//             OnError?.Invoke(this, error);
//         }
//
//         private void SendOnMessage(string message)
//         {
//             OnMessage?.Invoke(this, message);
//         }
//
//         private class WebSocketListenerEx : WebSocketListener
//         {
//             private readonly OkHttpWebSocket _webSocketConnection;
//
//
//             public WebSocketListenerEx(OkHttpWebSocket webSocketConnection)
//             {
//                 _webSocketConnection = webSocketConnection;
//             }
//
//             public override void OnOpen(IWebSocket webSocket, Response response)
//             {
//                 base.OnOpen(webSocket, response);
//                 _webSocketConnection.SendOnOpened();
//             }
//
//             public override void OnClosing(IWebSocket webSocket, int code, string reason)
//             {
//                 base.OnClosing(webSocket, code, reason);
//                 _webSocketConnection.SendOnOpened();
//             }
//
//             public override void OnClosed(IWebSocket webSocket, int code, string reason)
//             {
//                 base.OnClosed(webSocket, code, reason);
//                 _webSocketConnection.SendOnClose(code, reason);
//             }
//
//             public override void OnFailure(IWebSocket webSocket, Throwable t, Response response)
//             {
//                 base.OnFailure(webSocket, t, response);
//                 _webSocketConnection.SendOnError(new Exception(t.Message));
//             }
//
//             public override void OnMessage(IWebSocket webSocket, ByteString bytes)
//             {
//                 base.OnMessage(webSocket, bytes);
//             }
//
//             public override void OnMessage(IWebSocket webSocket, string text)
//             {
//                 base.OnMessage(webSocket, text);
//                 _webSocketConnection.SendOnMessage(text);
//             }
//         }
//     }
// }