using System;
using WebRTC.iOS.Binding;

namespace WebRTC.iOS.Demo
{
    public class ARDFileCaptureController
    {
        private readonly string[] Files =
        {
            "foreman.mp4",
            "SampleVideo_1280x720_10mb.mp4"
        };

        private readonly IFileVideoCaptureriOS _fileCapturer;
        private bool _hasStarted;
        private int _currentFile;

        public ARDFileCaptureController(IFileVideoCaptureriOS fileCapturer)
        {
            _fileCapturer = fileCapturer;
        }

        public void Toggle()
        {
            _currentFile = _currentFile == 0 ? 1 : 0;
            StopCapture();
            StartCapture();
        }

        public void StartCapture()
        {
            if (_hasStarted)
                return;

            _hasStarted = true;
            _fileCapturer.StartCapturingFromFileNamed(Files[_currentFile]);
        }

        public void StopCapture()
        {
            _hasStarted = false;
            _fileCapturer.StopCapture();
        }
    }
}
