namespace WebRTC.Abstraction
{
    public interface IRtpReceiver:INativeObject
    {
        string Id { get; }
        
        IMediaStreamTrack Track { get; }
    }
}