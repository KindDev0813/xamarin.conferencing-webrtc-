namespace WebRTC.Abstraction
{
    public interface IRtpSender:INativeObject
    {
        string SenderId { get; }
        
        IMediaStreamTrack Track { get;  }
        
    }
}