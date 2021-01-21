using System;
using System.Collections.Generic;
using System.Linq;
using Org.Webrtc;
using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    internal class RtpParametersNative : NativeObjectBase, IRtpParameters
    {
        private readonly RtpParameters _parameters;

        public RtpParametersNative(RtpParameters parameters) : base(parameters)
        {
            _parameters = parameters;
        }

        public string TransactionId
        {
            get => _parameters.TransactionId;
            set => _parameters.TransactionId = value;
        }

        public IRtpEncodingParameters[] Encodings => GetEncondingsInternal();

        public IRtpHeaderExtension[] HeaderExtensions =>
            _parameters.HeaderExtensions.Select(h => new RtpHeaderExtensionNative(h)).Cast<IRtpHeaderExtension>()
                .ToArray();

        public IRtcpParameters Rtcp => new RtcpParameters(_parameters.GetRtcp());

        private IRtpEncodingParameters[] GetEncondingsInternal()
        {
            if (_parameters.Encodings == null)
                return Array.Empty<IRtpEncodingParameters>();
            var list = new List<IRtpEncodingParameters>();
            foreach (var encoding in _parameters.Encodings)
            {
                list.Add(new RtpEncodingParametersNative((RtpParameters.Encoding) encoding));
            }
            return list.ToArray();
        }
    }
}