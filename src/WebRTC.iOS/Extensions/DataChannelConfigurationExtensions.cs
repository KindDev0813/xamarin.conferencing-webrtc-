using WebRTC.Abstraction;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS.Extensions
{
    internal static class DataChannelConfigurationExtensions
    {
        public static RTCDataChannelConfiguration ToNative(this DataChannelConfiguration self)
        {
            return new RTCDataChannelConfiguration
            {
                IsOrdered = self.IsOrdered,
                MaxRetransmitTimeMs = self.MaxRetransmitTimeMs,
                MaxPacketLifeTime = self.MaxRetransmitTimeMs,
                MaxRetransmits = self.MaxRetransmits,
                IsNegotiated = self.IsNegotiated,
                ChannelId = self.Id,
                Protocol = self.Protocol
            };
        }
    }
}