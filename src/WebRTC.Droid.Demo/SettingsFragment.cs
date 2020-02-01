using Android.OS;
using Android.Support.V7.Preferences;

namespace WebRTC.Droid.Demo
{
    public class SettingsFragment : PreferenceFragmentCompat
    {
        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.preferences, rootKey);
        }
    }
}