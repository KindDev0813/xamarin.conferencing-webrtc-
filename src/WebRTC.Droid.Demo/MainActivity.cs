using Android.App;
using Android.OS;
using Android.Widget;

namespace WebRTC.Droid.Demo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            Xamarin.Essentials.Platform.Init(this, savedInstanceState); // add this line to your code, it may also be called: bundle

            H113Platform.Init(this);
            
            SetContentView(Resource.Layout.activity_main);

            var appRTCBtn = FindViewById<Button>(Resource.Id.btn_apprtc);
            var h113Btn = FindViewById<Button>(Resource.Id.btn_h113);

            appRTCBtn.Click += (sender, args) => StartActivity(typeof(ConnectActivity));
            h113Btn.Click += (sender, args) => StartActivity(typeof(LoginActivity));
        }
    }
}