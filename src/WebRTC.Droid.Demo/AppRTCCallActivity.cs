using Android.App;
using Android.Content;
using WebRTC.AppRTC;

namespace WebRTC.Droid.Demo
{
    [Activity]
    public class AppRTCCallActivity : CallActivityBase<RoomConnectionParameters,SignalingParameters,AppRTCController>
    {
        private const string ExtraRoomId = "room_id";
        private const string ExtraIsLoopback = "loopback";
        private const string ExtraRoomUrl = "room_url";

        public static Intent CreateIntent(Context context, string roomId, bool isLoopback, string roomUrl = "")
        {
            var intent = new Intent(context, typeof(AppRTCCallActivity));
            intent.PutExtra(ExtraRoomId, roomId);
            intent.PutExtra(ExtraIsLoopback, isLoopback);

            if (string.IsNullOrEmpty(roomUrl))
            {
                roomUrl = context.GetString(Resource.String.pref_room_server_url_default);
            }

            intent.PutExtra(ExtraRoomUrl, roomUrl);
            return intent;
        }

        protected override AppRTCController CreateEngine() => new AppRTCController(this);

        protected override void Connect(AppRTCController rtcController, Intent intent)
        {
            var roomId = intent.GetStringExtra(ExtraRoomId);
            var loopback = intent.GetBooleanExtra(ExtraIsLoopback, false);
            var roomUrl = intent.GetStringExtra(ExtraRoomUrl);

            var roomConnectionParam = new RoomConnectionParameters
            {
                RoomId = roomId,
                IsLoopback = loopback,
                RoomUrl = roomUrl
            };

            rtcController.Connect(roomConnectionParam);
        }

        protected override CallFragment CreateCallFragment(Intent intent)
        {
            var roomId = intent.GetStringExtra(ExtraRoomId);
            return CallFragment.Create(roomId, true, true);
        }
    }
}