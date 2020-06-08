using Android.OS;
using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid;
using IVideoCapturer = WebRTC.Abstraction.IVideoCapturer;

namespace WebRTC.H113.Droid
{
    public class VideoController : Java.Lang.Object, IAppClientEvents, RendererCommon.IRendererEvents
    {
        private readonly ConnectionParameters _connectionParameters;
        private readonly bool _frontCamera;

        private readonly AppClient _client;

        private readonly VideoRendererProxy _videoRendererProxy;

        private IVideoControllerListener _videoControllerListener;

        public VideoController(ConnectionParameters connectionParameters, bool frontCamera)
        {
            _connectionParameters = connectionParameters;
            _frontCamera = frontCamera;
            _client = new AppClient(this);
            _videoRendererProxy = new VideoRendererProxy();

            EglBase = EglBaseHelper.Create();
        }

        internal IEglBase EglBase { get; }

        public bool IsConnected => _client.ConnectionState == ConnectionState.Connected;

        public void Start(IVideoControllerListener videoControllerListener)
        {
            _videoControllerListener = null;

            Disconnect();

            _videoControllerListener = videoControllerListener;

            _client.Connect(_connectionParameters);
        }

        public void SwitchCamera()
        {
            _client.SwitchCamera();
        }

        public void ToggleAudio()
        {
            _client.SetAudioEnabled(!_client.IsAudioEnable);
        }

        public void ToggleVideo()
        {
            _client.SetVideoEnabled(!_client.IsVideoEnable);
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        internal void OnViewCreated(SurfaceViewRenderer surfaceViewRenderer)
        {
            _videoRendererProxy.Renderer = surfaceViewRenderer;
            surfaceViewRenderer.Init(EglBase.EglBaseContext, this);
        }

        internal void OnViewDestroyed()
        {
            _videoRendererProxy.Renderer = null;
        }

        void IAppClientEvents.OnPeerFactoryCreated(IPeerConnectionFactory factory)
        {

        }

        void IAppClientEvents.OnConnected()
        {
            _videoControllerListener.OnConnect();
        }

        void IAppClientEvents.OnDisconnect(DisconnectType disconnectType)
        {
            _videoControllerListener.OnDisconnect(disconnectType);
        }

        IVideoCapturer IAppClientEvents.
            CreateVideoCapturer(IPeerConnectionFactory factory, IVideoSource videoSource) =>
            factory.CreateCameraCapturer(videoSource, _frontCamera);

        async void IAppClientEvents.ReadyToStart()
        {
            var isAllowed = await _videoControllerListener.RequestCameraPermissionAsync();
            if (isAllowed)
                _client.StartVideoCall(_videoRendererProxy, null);
        }

        void IAppClientEvents.OnError(string description)
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

        void IAppClientEvents.ShowNotification(int type, string title, string message)
        {

        }
    }
}