namespace WebRTC.Abstraction
{
    public interface IRtpParameters : INativeObject
    {
        string TransactionId { get; set; }
        
        IRtpEncodingParameters[] Encodings { get; }
        
        IRtpHeaderExtension[] HeaderExtensions { get; }
        
        IRtcpParameters Rtcp { get; }
        
    }
}