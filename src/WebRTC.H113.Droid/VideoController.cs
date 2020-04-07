using System;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
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

        private WeakReference<IEglBaseContext> _wfEglContext;

        private SurfaceViewRenderer _surfaceViewRenderer;

        public VideoController(ConnectionParameters connectionParameters, bool frontCamera)
        {
            _connectionParameters = connectionParameters;
            _frontCamera = frontCamera;


            _controller = new H113Controller(this);

            _videoRendererProxy = new VideoRendererProxy();
        }


        public bool IsConnected => _controller.IsWebRTCConnected;

        internal View OnCreateView(Context context)
        {
            OnViewDestroyed();
            _surfaceViewRenderer = new SurfaceViewRenderer(context);
            _surfaceViewRenderer.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent);
            _surfaceViewRenderer.SetScalingType(RendererCommon.ScalingType.ScaleAspectFit);
            _surfaceViewRenderer.SetZOrderMediaOverlay(true);
            _surfaceViewRenderer.SetEnableHardwareScaler(true);
            _videoRendererProxy.Renderer = _surfaceViewRenderer;
            
            if (_wfEglContext != null && _wfEglContext.TryGetTarget(out var eglBaseContext))
                _surfaceViewRenderer.Init(eglBaseContext, null);

            return _surfaceViewRenderer;
        }

        internal void OnViewDestroyed()
        {
            _videoRendererProxy.Renderer = null;
            _surfaceViewRenderer?.Release();
        }

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

        void IAppRTCEngineEvents.OnPeerFactoryCreated(IPeerConnectionFactory factory)
        {
            var androidFactory = (IPeerConnectionFactoryAndroid) factory.NativeObject;
            _wfEglContext = new WeakReference<IEglBaseContext>(androidFactory.EglBaseContext);
            _surfaceViewRenderer?.Init(androidFactory.EglBaseContext, this);
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
                _controller.StartVideoCall(_videoRendererProxy, new VideoRendererProxy());
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