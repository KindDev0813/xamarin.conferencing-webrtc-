using System;
using Android.Content;
using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.AppRTC.Abstraction;
using WebRTC.Droid;
using IVideoCapturer = WebRTC.Abstraction.IVideoCapturer;

namespace WebRTC.H113.Droid
{
    public class VideoController : IAppRTCEngineEvents
    {
        private readonly ConnectionParameters _connectionParameters;
        private readonly bool _frontCamera;

        private readonly H113Controller _controller;

        private readonly VideoRendererProxyEx _videoRendererProxy;
        
        private IVideoControllerListener _videoControllerListener;

        private bool _audioEnable = true;
        private bool _videoEnable = true;
        public VideoController(Context context, ConnectionParameters connectionParameters, bool frontCamera)
        {
            _connectionParameters = connectionParameters;
            _frontCamera = frontCamera;

            FullScreenRenderer = new SurfaceViewRenderer(context);

            _controller = new H113Controller(this);

            _videoRendererProxy = new VideoRendererProxyEx
            {
                Renderer = FullScreenRenderer
            };
        }

        public SurfaceViewRenderer FullScreenRenderer { get; }

        public bool IsConnected => _controller.Connected;

        public bool AudioEnable
        {
            get => _audioEnable;
            set
            {
                if (_audioEnable == value)
                    return;
                _audioEnable = value;
                _controller.SetAudioEnabled(value);
            }
        }
        
        public bool VideoEnable
        {
            get => _videoEnable;
            set
            {
                if (_videoEnable == value)
                    return;
                _videoEnable = value;
                _controller.SetVideoEnabled(value);
            }
        }

        public void Start(IVideoControllerListener videoControllerListener)
        {
            _videoControllerListener = videoControllerListener;

            _videoRendererProxy.Callback = () =>
            {
                _videoControllerListener.OnFirstFrame();
                _videoRendererProxy.Callback = null;
            };
            
            _controller.Connect(_connectionParameters);
        }

        public void SwitchCamera()
        {
            _controller.SwitchCamera();
        }

        public void ToggleAudio()
        {
            AudioEnable = !AudioEnable;
        }

        public void ToggleVideo()
        {
            VideoEnable = !VideoEnable;
        }
        
        public void Disconnect()
        {
            if (!_controller.Connected)
                return;
            _controller.Disconnect();
        }

        void IAppRTCEngineEvents.OnPeerFactoryCreated(IPeerConnectionFactory factory)
        {
            var androidFactory = (IPeerConnectionFactoryAndroid) factory.NativeObject;
            FullScreenRenderer.Init(androidFactory.EglBaseContext, null);
        }

        void IAppRTCEngineEvents.OnDisconnect(DisconnectType disconnectType)
        {
            _videoControllerListener.OnDisconnect(disconnectType);
            _controller.Disconnect();
        }

        IVideoCapturer IAppRTCEngineEvents.
            CreateVideoCapturer(IPeerConnectionFactory factory, IVideoSource videoSource) =>
            factory.CreateCameraCapturer(videoSource, _frontCamera);

        async void IAppRTCEngineEvents.ReadyToStart()
        {
            var isAllowed = await _videoControllerListener.RequestCameraPermissionAsync();
            if (isAllowed)
                _controller.StartVideoCall(_videoRendererProxy, null);
        }

        void IAppRTCEngineEvents.OnError(string description)
        {
            _videoControllerListener.OnError(description);
        }

        private class VideoRendererProxyEx : VideoRendererProxy
        {
            public Action Callback { get; set; }
            
            public override void OnFrame(VideoFrame p0)
            {
                base.OnFrame(p0);
                Callback?.Invoke();
            }
        }
    }
}