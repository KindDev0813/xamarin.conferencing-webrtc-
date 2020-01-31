using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebRTC.AppRTC
{
    public enum MethodType
    {
        Post,
        Get,
        Delete
    }
    
    public delegate void AsyncHttpCallback(string response, string errorMessage);

    public class AsyncHttpURLConnection
    {
        private const string HttpOrigin = "https://appr.tc";

        private static readonly HttpClient HttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(8)
        };

        static AsyncHttpURLConnection()
        {
            HttpClient.DefaultRequestHeaders.Add("origin", HttpOrigin);
        }

        private readonly MethodType _methodType;
        private readonly Uri _url;
        private readonly string _message;
        private readonly AsyncHttpCallback _callback;


        private string _contentType;

        public AsyncHttpURLConnection(MethodType methodType, string url, string message, AsyncHttpCallback callback)
        {
            _methodType = methodType;
            _url = new Uri(url);
            _message = message;
            _callback = callback;
        }

        public void SetContentType(string contentType)
        {
            _contentType = contentType;
        }

        public void Send()
        {
            Task.Factory.StartNew(SendHttpMessageAsync);
        }

        private async Task SendHttpMessageAsync()
        {
            _contentType = _contentType ?? "text/plain; charset=utf-8";
            HttpResponseMessage responseMessage;

            switch (_methodType)
            {
                case MethodType.Post:
                    var content = new StringContent(_message, Encoding.UTF8, _contentType);
                    responseMessage = await HttpClient.PostAsync(_url, content);
                    break;
                case MethodType.Get:
                    responseMessage = await HttpClient.GetAsync(_url);
                    break;
                case MethodType.Delete:
                    responseMessage = await HttpClient.DeleteAsync(_url);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                _callback?.Invoke(null, $"Non-200 response to {_methodType} to URL: {_url}");
                return;
            }

            var responseContent = await responseMessage.Content.ReadAsStringAsync();
            _callback?.Invoke(responseContent, null);
        }
    }
}