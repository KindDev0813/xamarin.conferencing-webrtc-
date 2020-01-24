using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.AppRTC;
using IVideoCapturer = WebRTC.Abstraction.IVideoCapturer;
using IVideoSink = Org.Webrtc.IVideoSink;


namespace WebRTC.Droid.Demo
{
    [Activity]
    public class CallActivity : AppCompatActivity, IAppRTCClientListener
    {
        private SurfaceViewRenderer _fullscreenRenderer;
        private SurfaceViewRenderer _pipRenderer;

        private IVideoSink _localRenderer;
        private IVideoSink _remoteRenderer;


        private IVideoCapturer _videoCapturer;
        private IVideoTrack _localVideoTrack;
        private IVideoTrack _remoteVideoTrack;

        private AppRTCClient _appRTCClient;


        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.call_activity);

            _fullscreenRenderer = FindViewById<SurfaceViewRenderer>(Resource.Id.fullscreen_video_view);
            _pipRenderer = FindViewById<SurfaceViewRenderer>(Resource.Id.pip_video_view);


            var appRTCFactory = new AppRTCFactory(Application);

            _appRTCClient = new AppRTCClient(new AppRTCClientConfig(H113Constants.Token, H113Constants.WssUrl),
                appRTCFactory);

            _appRTCClient.Listener = this;

            await _appRTCClient.ConnectAsync(H113Constants.Phone);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _appRTCClient?.Disconnect();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void DidCreatePeerConnection(IPeerConnection peerConnection)
        {
            var nativeFactory = (IPeerConnectionFactoryAndroid) peerConnection.PeerConnectionFactory;
            _pipRenderer.Init(nativeFactory.EglBaseContext, null);
            _pipRenderer.SetScalingType(RendererCommon.ScalingType.ScaleAspectFit);

            _fullscreenRenderer.Init(nativeFactory.EglBaseContext, null);
            _fullscreenRenderer.SetScalingType(RendererCommon.ScalingType.ScaleAspectFit);
        }

        public void DidOpenDataChannel(IDataChannel dataChannel)
        {
        }

        public void DidChangeState(AppClientState state)
        {
        }

        public void DidReceiveLocalVideoTrack(IVideoTrack videoTrack)
        {
            _localVideoTrack = videoTrack;

            _localVideoTrack.AddRenderer(new VideoRendererProxy
            {
                VideoSink = _pipRenderer
            });
        }

        public void DidReceiveRemoteVideoTrack(IVideoTrack videoTrack)
        {
        }

        public void DidCreateCapturer(IVideoCapturer videoCapturer)
        {
            _videoCapturer = videoCapturer;
            _videoCapturer.StartCapture(0, 0, 0);
        }

        public void DidRegisterWithCollider()
        {
        }

        public void OnError(AppRTCException error)
        {
        }
    }
}