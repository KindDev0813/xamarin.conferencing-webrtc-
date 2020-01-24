using System.Collections.Generic;
using System.Linq;
using Org.Webrtc;
using WebRTC.Abstraction;

namespace WebRTC.Droid.Extensions
{
    internal static class IceServerExtensions
    {
        public static PeerConnection.IceServer ToNative(this IceServer self)
        {
            return PeerConnection.IceServer.InvokeBuilder(self.Urls)
                .SetUsername(self.Username)
                .SetPassword(self.Password)
                .SetTlsCertPolicy(self.TlsCertPolicy.ToNative())
                .CreateIceServer();
        }
        
        public static IEnumerable<PeerConnection.IceServer> ToNative(this IEnumerable<IceServer> self)
        {
            return self.Select(ToNative);
        }
    }
}