using System;
using WebRTC.H113;

namespace WebRTC.iOS.Demo
{
    public class H113CallViewController : CallViewControllerBase<ConnectionParameters,RegisteredMessage, H113Controller>
    {
        protected override void Connect(H113Controller rtcController)
        {
            throw new NotImplementedException();
        }

        protected override H113Controller CreateController() 
        {
            throw new NotImplementedException();
        }
    }
}
