using System.Linq;
using Org.Webrtc;

namespace WebRTC.Droid.Extensions
{
    internal static class MediaConstraintsExtensions
    {
        public static MediaConstraints ToNative(this Abstraction.MediaConstraints self)
        {
            var optionals = self.Optional.Select(p => new MediaConstraints.KeyValuePair(p.Key, p.Value)).ToList();
            var mandatory = self.Mandatory.Select(p => new MediaConstraints.KeyValuePair(p.Key, p.Value)).ToList();
            return new MediaConstraints
            {
                Mandatory = mandatory,
                Optional = optionals
            };
        }
    }
}