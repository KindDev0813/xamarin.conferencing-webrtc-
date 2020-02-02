using System;
using WebRTC.Abstraction;
using Org.Webrtc;
using WebRTC.Droid.Extensions;
using ISdpObserver = Org.Webrtc.ISdpObserver;
using SessionDescription = Org.Webrtc.SessionDescription;

namespace WebRTC.Droid
{
    public class SdpObserverProxy : Java.Lang.Object, ISdpObserver
    {
        private readonly Abstraction.ISdpObserver _observer;

        public SdpObserverProxy(Abstraction.ISdpObserver observer)
        {
            _observer = observer;
        }

        public void OnCreateFailure(string p0)
        {
            _observer?.OnCreateFailure(p0);
        }

        public void OnCreateSuccess(SessionDescription p0)
        {
            _observer?.OnCreateSuccess(p0.ToNet());
        }

        public void OnSetFailure(string p0)
        {
           _observer.OnSetFailure(p0);
        }

        public void OnSetSuccess()
        {
            _observer.OnSetSuccess();
        }
    }
}