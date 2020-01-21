namespace WebRTC.Abstraction
{
    public interface IVideoCapturer : INativeObject
    {
        void StartCapture(int videoWidth, int videoHeight, int fps);
        void StopCapture();
    }
    
}