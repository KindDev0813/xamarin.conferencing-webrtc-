namespace WebRTC.Abstraction
{
    public interface IVideoCapturer
    {
        void StartCapture(int videoWidth, int videoHeight, int fps);
        void StopCapture();
    }
    
}