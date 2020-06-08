using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace H113.Demo.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.activity_splash);

            var mainActivity = FindViewById<Button>(Resource.Id.btn_main_activity);
            mainActivity.Click += (sender, args) => StartActivity(typeof(MainActivity));
            
            var mainActivityConfigurationChange = FindViewById<Button>(Resource.Id.btn_main_activity_configuration);
            mainActivityConfigurationChange.Click += (sender, args) => StartActivity(typeof(MainActivityWithConfiguration));
        }
    }
}