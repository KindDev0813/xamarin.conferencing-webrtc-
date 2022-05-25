using Foundation;
using WebRTC.Abstraction;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS
{
    internal class RtpEncodingParametersNative : NativeObjectBase, IRtpEncodingParameters
    {
        private readonly RTCRtpEncodingParameters _parameters;

        public RtpEncodingParametersNative(RTCRtpEncodingParameters parameters):base(parameters)
        {
            _parameters = parameters;
        }

        public bool IsActive
        {
            get => _parameters.IsActive;
            set => _parameters.IsActive = value;
        }

        public int? MaxBitrateBps
        {
            get => _parameters.MaxBitrateBps?.Int32Value;
            set => _parameters.MaxBitrateBps =  value.HasValue ? new NSNumber(value.Value) : null;
        }
        
        public int? MinBitrateBps
        { 
            get => _parameters.MinBitrateBps?.Int32Value;
            set => _parameters.MinBitrateBps =  value.HasValue ? new NSNumber(value.Value) : null;
        }

        public int? MaxFrameRate
        {
            get => _parameters.MaxFramerate?.Int32Value;
            set => _parameters.MaxFramerate =  value.HasValue ? new NSNumber(value.Value) : null;
        }

        public double NetworkPriority
        {
            get => _parameters.NetworkPriority;
            set => _parameters.NetworkPriority = value;
        }

        public int? NumTemporalLayers
        {
            get => _parameters.NumTemporalLayers?.Int32Value;
            set => _parameters.NumTemporalLayers =  value.HasValue ? new NSNumber(value.Value) : null;
        }

        public double? ScaleResolutionDownBy
        {
            get => _parameters.ScaleResolutionDownBy?.DoubleValue;
            set => _parameters.ScaleResolutionDownBy =  value.HasValue ? new NSNumber(value.Value) : null;
        }

        public long? Ssrc=> _parameters.Ssrc?.Int32Value;
       
    }
}