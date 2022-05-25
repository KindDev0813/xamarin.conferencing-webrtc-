namespace WebRTC.Abstraction
{
    public interface IRtpTransceiver : INativeObject
    {
        RtpMediaType MediaType { get; }
        string Mid { get; }
        bool IsStopped { get; }
        RtpTransceiverDirection Direction { get; }
        IRtpSender? Sender { get; }
        IRtpReceiver? Receiver { get; }
        void Stop();
    }
}