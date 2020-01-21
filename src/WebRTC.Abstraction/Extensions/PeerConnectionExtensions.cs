namespace WebRTC.Abstraction.Extensions
{
    public static class PeerConnectionExtensions
    {
        public static IRtpTransceiver GetVideoTransceiver(this IPeerConnection self)
        {
            foreach (var transceiver in self.Transceivers)
            {
                if (transceiver.MediaType == RtpMediaType.Video)
                    return transceiver;
            }
            return null;
        }

    }
}