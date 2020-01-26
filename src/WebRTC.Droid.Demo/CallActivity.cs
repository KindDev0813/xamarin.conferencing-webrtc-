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
    public class CallActivity : AppCompatActivity
    {
        private SurfaceViewRenderer _fullscreenRenderer;
        private SurfaceViewRenderer _pipRenderer;

        private IVideoSink _localRenderer;
        private IVideoSink _remoteRenderer;
   


        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.call_activity);

            _fullscreenRenderer = FindViewById<SurfaceViewRenderer>(Resource.Id.fullscreen_video_view);
            _pipRenderer = FindViewById<SurfaceViewRenderer>(Resource.Id.pip_video_view);


           
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}