using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using WebRTC.AppRTC;

namespace WebRTC.Droid.Demo
{
    [Activity]
    public class ConnectActivity : BaseActivity
    {
        private EditText _roomEditText;
        private ImageButton _connectButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_connect);

            _roomEditText = FindViewById<EditText>(Resource.Id.room_edittext);

            _connectButton = FindViewById<ImageButton>(Resource.Id.connect_button);

            _connectButton.Click += (sender, args) =>
            {
                var intent = AppRTCCallActivity.CreateIntent(this, _roomEditText.Text, false);
                StartActivity(intent);
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
            _roomEditText.Text = GenerateRoom.GenerateRoomName();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.connect_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_settings)
            {
                StartActivity(typeof(SettingsActivity));
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}