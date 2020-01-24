namespace WebRTC.Abstraction
{
    public interface IVideoCapturer : INativeObject
    {
        bool IsScreencast { get; }
        void StartCapture(int videoWidth, int videoHeight, int fps);
        void StopCapture();
    }
}