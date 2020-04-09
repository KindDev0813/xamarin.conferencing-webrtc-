using Android.App;
using Android.Content.PM;

namespace H113.Demo.Droid
{
    [Activity(ScreenOrientation = ScreenOrientation.Sensor,ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize)]
    public class MainActivityWithConfiguration : BaseMainActivity
    {
        
    }
}