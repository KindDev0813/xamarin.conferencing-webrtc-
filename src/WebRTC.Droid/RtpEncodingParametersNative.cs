using Org.Webrtc;
using WebRTC.Abstraction;

namespace WebRTC.Droid
{
    internal class RtpEncodingParametersNative : NativeObjectBase, IRtpEncodingParameters
    {
        private readonly RtpParameters.Encoding _parameters;

        public RtpEncodingParametersNative(RtpParameters.Encoding parameters):base(parameters)
        {
            _parameters = parameters;
        }

        public bool IsActive
        {
            get => _parameters.Active;
            set => _parameters.Active = value;
        }

        public int? MaxBitrateBps
        {
            get => _parameters.MaxBitrateBps?.IntValue();
            set => _parameters.MaxBitrateBps =  value.HasValue ? new Java.Lang.Integer(value.Value) : null;
        }
        
        public int? MinBitrateBps
        { 
            get => _parameters.MinBitrateBps?.IntValue();
            set => _parameters.MinBitrateBps =   value.HasValue ? new Java.Lang.Integer(value.Value) : null;
        }

        public int? MaxFrameRate
        {
            get => _parameters.MaxFramerate?.IntValue();
            set => _parameters.MaxFramerate =  value.HasValue ? new Java.Lang.Integer(value.Value) : null;
        }

        public double NetworkPriority
        {
            get => 0;
            set {}
        }

        public int? NumTemporalLayers
        {
            get => _parameters.NumTemporalLayers?.IntValue();
            set => _parameters.NumTemporalLayers =   value.HasValue ? new Java.Lang.Integer(value.Value) : null;
        }

        public double? ScaleResolutionDownBy
        {
            get => _parameters.ScaleResolutionDownBy?.DoubleValue();
            set => _parameters.ScaleResolutionDownBy =   value.HasValue ? new Java.Lang.Double(value.Value) : null;
        }

        public long? Ssrc=> _parameters.Ssrc?.IntValue();
    }
}