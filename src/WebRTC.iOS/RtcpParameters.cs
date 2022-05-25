using WebRTC.Abstraction;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS
{
    internal class RtcpParameters : NativeObjectBase, IRtcpParameters
    {
        private readonly RTCRtcpParameters _parameters;

        public RtcpParameters(RTCRtcpParameters parameters) : base(parameters)
        {
            _parameters = parameters;
        }
    }
}