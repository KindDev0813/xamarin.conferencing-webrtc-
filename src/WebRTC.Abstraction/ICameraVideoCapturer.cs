namespace WebRTC.Abstraction
{
    public interface ICameraVideoCapturer : IVideoCapturer
    {
        void SwitchCamera();
        void SwitchCamera(RTCCameraDevice device);
    }
}