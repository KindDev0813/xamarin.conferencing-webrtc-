using Android.App;
using Android.Content;
using WebRTC.AppRTC.Abstraction;
using WebRTC.H113;

namespace WebRTC.Droid.Demo
{
        
    [Activity]
    public class H113CallActivity : CallActivityBase<H113Engine>
    {
        protected override H113Engine CreateEngine() => new H113Engine(this);
       

        protected override void Connect(H113Engine rtcEngine,Intent intent)
        {
            rtcEngine.Connect(new ConnectionParameters("","","",0,0));
        }

        protected override CallFragment CreateCallFragment(Intent intent) => CallFragment.Create("",true,true);
    }
}