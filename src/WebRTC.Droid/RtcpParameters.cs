using Org.Webrtc;
using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    internal class RtcpParameters : NativeObjectBase, IRtcpParameters
    {
        private readonly RtpParameters.Rtcp _parameters;

        public RtcpParameters(RtpParameters.Rtcp parameters) : base(parameters)
        {
            _parameters = parameters;
        }
    }
}