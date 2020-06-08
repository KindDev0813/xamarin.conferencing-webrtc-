using WebRTC.AppRTC.Abstraction;

namespace WebRTC.H113
{
    public class ConnectionParameters : IConnectionParameters
    {
        public ConnectionParameters(string wsUrl, string protocol, string phone)
        {
            WsUrl = wsUrl;
            Protocol = protocol;
            Phone = phone;
        }

        public string WsUrl { get; }
        public string Protocol { get; }
        public string Phone { get; }
    }
}