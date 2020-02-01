using System;
using WebRTC.Abstraction;
using Org.Webrtc;
using WebRTC.Droid.Extensions;
using SessionDescription = Org.Webrtc.SessionDescription;

namespace WebRTC.Droid
{
    public class SdpObserverProxy : Java.Lang.Object, ISdpObserver
    {
        private readonly SdpCompletionHandler _completionHandler;

        public SdpObserverProxy(SdpCompletionHandler completionHandler)
        {
            _completionHandler = completionHandler;
        }

        public void OnCreateFailure(string p0)
        {
            _completionHandler?.Invoke(null, new Exception(p0));
        }

        public void OnCreateSuccess(SessionDescription p0)
        {
             _completionHandler?.Invoke(p0.ToNet(), null);
        }

        public void OnSetFailure(string p0)
        {
           _completionHandler?.Invoke(null, new Exception(p0));
        }

        public void OnSetSuccess()
        {
            _completionHandler?.Invoke(null, null);
        }
    }
}