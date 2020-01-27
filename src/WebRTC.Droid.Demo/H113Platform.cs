using System;
using Android.App;
using Android.OS;
using Java.Lang;
using Square.OkHttp3;
using Square.OkIO;
using WebRTC.AppRTC;
using Exception = System.Exception;



namespace WebRTC.Droid.Demo
{
    public static class H113Platform
    {
        public static void Init(Activity activity)
        {
            Platform.Init(activity);
            WebSocketConnectionFactory.Factory = () => new WebSocketConnection();
            ExecutorServiceFactory.Factory = tag => new ExecutorServiceImpl(tag);
            ExecutorServiceFactory.MainExecutor = new MainExecutor();
        }

        private class MainExecutor : AppRTC.IExecutor
        {
            private readonly Handler _handler = new Handler(Looper.MainLooper);
            public bool IsCurrentExecutor => Looper.MainLooper == Looper.MyLooper();
            public void Execute(Action action) => _handler.Post(action);
        }

        private class ExecutorServiceImpl : IExecutorService
        {

            private readonly HandlerThread _handlerThread;
            private readonly Handler _handler;

            public ExecutorServiceImpl(string tag)
            {
                _handlerThread = new HandlerThread(tag);
                _handlerThread.Start();
                _handler = new Handler(_handlerThread.Looper);
            }


            public bool IsCurrentExecutor => _handlerThread.Looper == Looper.MyLooper();
            public void Execute(Action action) => _handler.Post(action);
        
            public void Release()
            {
                _handlerThread.QuitSafely();
            }
        }
        
        private class WebSocketConnection : IWebSocketConnection
        {
            private readonly WebSocketListenerEx _listener;
            private IWebSocket _webSocket;

            public WebSocketConnection()
            {
                _listener = new WebSocketListenerEx(this);
            }

            public event EventHandler OnOpened;
            public event EventHandler<(int code, string reason)> OnClosed;
            public event EventHandler<Exception> OnError;
            public event EventHandler<string> OnMessage;

            public void Dispose()
            {
                _webSocket.Dispose();
                _listener.Dispose();
            }

            public void Open(string url, string protocol = null, string authToken = null)
            {
                var requestBuilder = new Request.Builder()
                    .Url(url);
                if (!string.IsNullOrEmpty(protocol))
                {
                    requestBuilder.AddHeader("Sec-WebSocket-Protocol", protocol);
                }

                var request = requestBuilder.Build();

                var client = new OkHttpClient();

                _webSocket = client.NewWebSocket(request, _listener);
            }

            public void Close()
            {
                _webSocket.Close(1000, null);
            }

            public void Send(string message)
            {
                _webSocket.Send(message);
            }

            private void SendOnOpened()
            {
                OnOpened?.Invoke(this, EventArgs.Empty);
            }

            private void SendOnClose(int code, string reason)
            {
                OnClosed?.Invoke(this, (code, reason));
            }

            private void SendOnError(Exception error)
            {
                OnError?.Invoke(this, error);
            }

            private void SendOnMessage(string message)
            {
                OnMessage?.Invoke(this, message);
            }

            private class WebSocketListenerEx : WebSocketListener
            {
                private readonly WebSocketConnection _webSocketConnection;


                public WebSocketListenerEx(WebSocketConnection webSocketConnection)
                {
                    _webSocketConnection = webSocketConnection;
                }

                public override void OnOpen(IWebSocket webSocket, Response response)
                {
                    base.OnOpen(webSocket, response);
                    _webSocketConnection.SendOnOpened();
                }

                public override void OnClosing(IWebSocket webSocket, int code, string reason)
                {
                    base.OnClosing(webSocket, code, reason);
                    _webSocketConnection.SendOnOpened();
                }

                public override void OnClosed(IWebSocket webSocket, int code, string reason)
                {
                    base.OnClosed(webSocket, code, reason);
                    _webSocketConnection.SendOnClose(code,reason);
                }

                public override void OnFailure(IWebSocket webSocket, Throwable t, Response response)
                {
                    base.OnFailure(webSocket, t, response);
                    _webSocketConnection.SendOnError(new Exception(t.Message));
                }

                public override void OnMessage(IWebSocket webSocket, ByteString bytes)
                {
                    base.OnMessage(webSocket, bytes);
                }

                public override void OnMessage(IWebSocket webSocket, string text)
                {
                    base.OnMessage(webSocket, text);
                    _webSocketConnection.SendOnMessage(text);
                }
            }
        }
    }
}