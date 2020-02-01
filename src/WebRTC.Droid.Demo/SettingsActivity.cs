using Android.App;
using Android.Content;
using Android.OS;

namespace WebRTC.Droid.Demo
{
    [Activity]
    public class SettingsActivity : BaseActivity,ISharedPreferencesOnSharedPreferenceChangeListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var settingsFragment = new SettingsFragment();
            SupportFragmentManager.BeginTransaction()
                .Replace(Android.Resource.Id.Content, settingsFragment)
                .Commit();
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            
        }
    }
}