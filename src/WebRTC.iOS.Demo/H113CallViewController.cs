using System;
using WebRTC.H113;

namespace WebRTC.iOS.Demo
{
    public class H113CallViewController : CallViewControllerBase<H113Engine>
    {
        protected override void Connect(H113Engine rtcEngine)
        {
            throw new NotImplementedException();
        }

        protected override H113Engine CreateEngine() 
        {
            throw new NotImplementedException();
        }
    }
}
