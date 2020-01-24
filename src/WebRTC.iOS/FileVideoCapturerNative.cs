using System;
using System.Diagnostics;
using WebRTC.Abstraction;

namespace WebRTC.iOS
{
    internal class FileVideoCapturerNative : NativeObjectBase,IFileVideoCapturer
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
            _capturer.StartCapturingFromFileNamed(_file,(err)=>Debug.WriteLine($"FileVideoCapturerNative failed:{err}"));
        }

        public void StartCapture(int videoWidth, int videoHeight, int fps)
        {
            StartCapture();
        }

        public void StopCapture()
        {
            _capturer.StopCapture();
        }
    }
}