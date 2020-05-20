using Android.App;
using Android.Content.PM;
using Android.OS;
using WebRTC.AppRTC.Abstraction;
using WebRTC.H113.Droid;

namespace H113.Demo.Droid
{
    [Activity(ScreenOrientation = ScreenOrientation.Sensor)]
    public class MainActivity : BaseMainActivity
    {
        private const string IsConnectedExtra = "is_connected";

        private bool _isConnected;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // if savedInstanceState != null means we are recreating the activity
            if (savedInstanceState != null)
            {
                var fragment = SupportFragmentManager.FindFragmentByTag("video_fragment") as VideoFragment;
                fragment?.GetVideoControllerAsync(this);
            }
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            _isConnected = savedInstanceState.GetBoolean(IsConnectedExtra, false);
            HideVideoContainer(!_isConnected);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutBoolean(IsConnectedExtra, _isConnected);
        }

        public override void OnConnect(DisconnectType disconnectType)
        {
            base.OnConnect(disconnectType);
            _isConnected = true;
        }

        public override void OnDisconnect(DisconnectType disconnectType)
        {
            base.OnDisconnect(disconnectType);
            _isConnected = false;
        }


    }
}