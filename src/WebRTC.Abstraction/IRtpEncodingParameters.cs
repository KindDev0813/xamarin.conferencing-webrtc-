namespace WebRTC.Abstraction
{
    public interface IRtpEncodingParameters
    {
        bool IsActive { get; set; }
        
        int? MaxBitrateBps { get; set; }
        int? MinBitrateBps { get; set; }
        int? MaxFrameRate { get; set; }
        
        double NetworkPriority { get; set; }
        
        int? NumTemporalLayers { get; set; }
        
        double? ScaleResolutionDownBy { get; set; }
        
        long? Ssrc { get; }
    }
}