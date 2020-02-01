using System;
using System.Diagnostics;
using WebRTC.Abstraction;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS
{
    public interface IFileVideoCaptureriOS : IFileVideoCapturer
    {
        void StartCapturingFromFileNamed(string fileName);
    }

    internal class FileVideoCapturerNative : NativeObjectBase, IFileVideoCaptureriOS
    {
        private readonly RTCFileVideoCapturer _capturer;
        private readonly string _file;


        public FileVideoCapturerNative(RTCFileVideoCapturer capturer,string file):base(capturer)
        {
            _capturer = capturer;
            _file = file;
        }

        public bool IsScreencast => false;
        
        public void StartCapture()
        {
            StartCapturingFromFileNamed(_file);
        }

        public void StartCapture(int videoWidth, int videoHeight, int fps)
        {
            StartCapture();
        }

        public void StartCapturingFromFileNamed(string fileName)
        {
            _capturer.StartCapturingFromFileNamed(_file, (err) => Debug.WriteLine($"FileVideoCapturerNative failed:{err}"));
        }

        public void StopCapture()
        {
            _capturer.StopCapture();
        }
    }
}