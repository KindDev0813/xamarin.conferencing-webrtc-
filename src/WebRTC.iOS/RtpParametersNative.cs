using System;
using System.Linq;
using WebRTC.Abstraction;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS
{
    internal class RtpParametersNative : NativeObjectBase, IRtpParameters
    {
        private readonly RTCRtpParameters _parameters;

        public RtpParametersNative(RTCRtpParameters parameters) : base(parameters)
        {
            _parameters = parameters;
        }

        public string TransactionId
        {
            get => _parameters.TransactionId;
            set => _parameters.TransactionId = value;
        }

        public IRtpEncodingParameters[] Encodings => _parameters.Encodings == null
            ? Array.Empty<IRtpEncodingParameters>()
            : _parameters.Encodings.Select(e => new RtpEncodingParametersNative(e)).Cast<IRtpEncodingParameters>()
                .ToArray();

        public IRtpHeaderExtension[] HeaderExtensions =>
            _parameters.HeaderExtensions.Select(h => new RtpHeaderExtensionNative(h)).Cast<IRtpHeaderExtension>()
                .ToArray();

        public IRtcpParameters Rtcp => new RtcpParameters(_parameters.Rtcp);
    }
}