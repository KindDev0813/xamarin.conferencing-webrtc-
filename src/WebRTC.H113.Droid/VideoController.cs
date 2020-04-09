using Android.OS;
using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.AppRTC.Abstraction;
using WebRTC.Droid;
using IVideoCapturer = WebRTC.Abstraction.IVideoCapturer;

namespace WebRTC.H113.Droid
{
    public class VideoController : Java.Lang.Object, IAppRTCEngineEvents, RendererCommon.IRendererEvents
    {
        private readonly ConnectionParameters _connectionParameters;
        private readonly bool _frontCamera;

        private readonly H113Controller _controller;

        private readonly VideoRendererProxy _videoRendererProxy;
        
        private IVideoControllerListener _videoControllerListener;

        public VideoController(ConnectionParameters connectionParameters, bool frontCamera)
        {
            _connectionParameters = connectionParameters;
            _frontCamera = frontCamera;
            _controller = new H113Controller(this);
            _videoRendererProxy = new VideoRendererProxy();

            EglBase = EglBaseHelper.Create();
        }

        internal IEglBase EglBase { get; }

        public bool IsConnected => _controller.IsWebRTCConnected;
        
        public void Start(IVideoControllerListener videoControllerListener)
        {
            _videoControllerListener = null;

            Disconnect();

            _videoControllerListener = videoControllerListener;

            _controller.Connect(_connectionParameters);
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
            _controller.Disconnect();
        }

        internal void OnViewCreated(SurfaceViewRenderer surfaceViewRenderer)
        {
            _videoRendererProxy.Renderer = surfaceViewRenderer;
            surfaceViewRenderer.Init(EglBase.EglBaseContext,this);
        }

        internal void OnViewDestroyed()
        {
            _videoRendererProxy.Renderer = null;
        }

        void IAppRTCEngineEvents.OnPeerFactoryCreated(IPeerConnectionFactory factory)
        {
          
        }

        public void OnConnect()
        {
            _videoControllerListener.OnConnect();
        }

        void IAppRTCEngineEvents.OnDisconnect(DisconnectType disconnectType)
        {
            _videoControllerListener.OnDisconnect(disconnectType);
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

        void RendererCommon.IRendererEvents.OnFirstFrameRendered()
        {
            var handler = new Handler(Looper.MainLooper);
            handler.Post(() => _videoControllerListener?.OnFirstFrame());
        }

        void RendererCommon.IRendererEvents.OnFrameResolutionChanged(int width, int height, int rotation)
        {
            
        }
    }
}