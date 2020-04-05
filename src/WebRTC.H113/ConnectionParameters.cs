using WebRTC.AppRTC.Abstraction;

namespace WebRTC.H113
{
    public class ConnectionParameters : IConnectionParameters
    {
        public ConnectionParameters(string wsUrl, string protocol, string phone, double lat, double log)
        {
            WsUrl = wsUrl;
            Protocol = protocol;
            Phone = phone;
            Lat = lat;
            Log = log;
        }

        public string WsUrl { get; }
        public string Protocol { get; }
        public string Phone { get; }
        public double Lat { get; }
        public double Log { get; }
    }
}