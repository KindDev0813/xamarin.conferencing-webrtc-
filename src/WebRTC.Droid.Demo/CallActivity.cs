using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using WebRTC.Abstraction;
using WebRTC.AppRTC;


namespace WebRTC.Droid.Demo
{
    [Activity]
    public class CallActivity : AppCompatActivity, IAppRTCClientListener
    {
        private AppRTCClient _appRTCClient;
        private VideoCapturerFactory _videoCapturerFactory;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            AppRTC.AppRTC.Init(new AppRTCFactory());

            _videoCapturerFactory = new VideoCapturerFactory(this);

            _appRTCClient = new AppRTCClient(new AppRTCClientConfig(H113Constants.Token, H113Constants.WssUrl),
                _videoCapturerFactory);

            _appRTCClient.Listener = this;
            
            _appRTCClient.ConnectAsync(H113Constants.Phone);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _appRTCClient?.DisconnectAsync();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void DidCreatePeerConnection(IPeerConnection peerConnection)
        {
            
        }

        public void DidOpenDataChannel(IDataChannel dataChannel)
        {
        }

        public void DidChangeState(AppClientState state)
        {
        }

        public void DidReceiveLocalVideoTrack(IVideoTrack videoTrack)
        {
        }

        public void DidReceiveRemoteVideoTrack(IVideoTrack videoTrack)
        {
        }

        public void DidCreateCapturer(IVideoCapturer videoCapturer)
        {
        }

        public void DidRegisterWithCollider()
        {
        }

        public void OnError(AppRTCException error)
        {
        }
    }
}