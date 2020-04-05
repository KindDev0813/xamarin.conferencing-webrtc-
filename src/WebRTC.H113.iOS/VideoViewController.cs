using System;

using UIKit;
using WebRTC.Abstraction;
using WebRTC.AppRTC.Abstraction;
using WebRTC.iOS;

namespace WebRTC.H113.iOS
{


    public partial class VideoViewController : UIViewController, IAppRTCEngineEvents
    {
        private readonly VideoRendererProxy _localRenderer = new VideoRendererProxy();


        private readonly H113Controller _controller;

        private ConnectionParameters _connectionParameters;
        private IVideoControllerListener _videoControllerListener;
        

        public VideoViewController() : base("VideoViewController", null)
        {
            _controller = new H113Controller(this);
        }

        public FileVideoCapturer FileVideoCapturer { get; set; }

        public bool UseFrontCamera { get; set; } = true;


        public void Start(IVideoControllerListener videoControllerListener, ConnectionParameters connectionParameters)
        {
            _videoControllerListener = null;
            if (_controller.Connected)
                _controller.Disconnect();
            _videoControllerListener = videoControllerListener;
            _controller.Connect(connectionParameters);
        }

        public void SwitchCamera()
        {
            _controller.SwitchCamera();
        }

        public void ToggleAudio()
        {
            _controller.SetAudioEnabled(!_controller.IsAudioEnable);
        }

        public void ToggleVideo()
        {
            _controller.SetVideoEnabled(!_controller.IsVideoEnable);
        }

        public void Disconnect()
        {
            if (!_controller.Connected)
                return;
            _controller.Disconnect();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        void IAppRTCEngineEvents.OnPeerFactoryCreated(IPeerConnectionFactory factory)
        {

        }

        void IAppRTCEngineEvents.OnDisconnect(DisconnectType disconnectType)
        {
            _videoControllerListener.OnDisconnect(disconnectType);
            _controller.Disconnect();
        }

        IVideoCapturer IAppRTCEngineEvents.CreateVideoCapturer(IPeerConnectionFactory factory, IVideoSource videoSource)
        {
            if (FileVideoCapturer != null)
                return FileVideoCapturer;
            return factory.CreateCameraCapturer(videoSource, UseFrontCamera);
        }

        async void IAppRTCEngineEvents.ReadyToStart()
        {
            var isAllowed = await _videoControllerListener.RequestCameraPermissionAsync();
            if (isAllowed)
                _controller.StartVideoCall(_localRenderer, null);
        }

        void IAppRTCEngineEvents.OnError(string description)
        {
            _videoControllerListener.OnError(description);
        }
    }
}

