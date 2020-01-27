using System.Collections.Generic;
using WebRTC.Abstraction;

namespace WebRTC.AppRTC.Extensions
{
    public static class RegisteredMessageExtensions
    {
        public static IceServer[] GetIceServers(this RegisteredMessage self)
        {
            var list = new List<IceServer>();
            if (!string.IsNullOrEmpty(self.StunServer))
                list.Add(new IceServer(self.StunServer));
            if (self.RTCServer?.Turn1 != null)
                list.Add(CreateIceServer(self.RTCServer.Turn1));
            if (self.RTCServer?.Turn2 != null)
                list.Add(CreateIceServer(self.RTCServer.Turn2));
            return list.ToArray();
        }

        private static IceServer CreateIceServer(RegisteredMessage.RTCServerEx server)
        {
            return new IceServer(server.Url, server.Username, server.Password);
        }
    }
}