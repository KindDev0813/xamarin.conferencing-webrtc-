using System;
using WebRTC.H113;

namespace WebRTC.iOS.Demo
{
    public class H113CallViewController : CallViewControllerBase<ConnectionParameters, RegisteredMessage, H113Controller>
    {
        protected override void Connect(H113Controller rtcController)
        {
            rtcController.Connect(new ConnectionParameters(H113Constants.WssUrl, H113Constants.Token, "98056391", 54.23,
                          12.12));
        }

        protected override H113Controller CreateController() => new H113Controller(this);
    }
}
