using Android.App;
using Android.Content;
using WebRTC.AppRTC.Abstraction;
using WebRTC.H113;

namespace WebRTC.Droid.Demo
{
    [Activity]
    public class H113CallActivity : CallActivityBase<ConnectionParameters, RegisteredMessage, H113Controller>
    {
        protected override H113Controller CreateEngine() => new H113Controller(this);


        protected override void Connect(H113Controller rtcController, Intent intent)
        {
            rtcController.Connect(new ConnectionParameters(H113Constants.WssUrl, H113Constants.Token, "98056391", 54.23,
                12.12));
        }

        protected override CallFragment CreateCallFragment(Intent intent) => CallFragment.Create("", true, true);
    }
}