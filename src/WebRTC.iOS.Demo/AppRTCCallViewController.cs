using System;
using WebRTC.AppRTC;

namespace WebRTC.iOS.Demo
{
    public class AppRTCCallViewController : CallViewControllerBase<AppRTCEngine>
    {
        private readonly string _roomId;
        private readonly bool _isLoopback;

        public AppRTCCallViewController(string roomId,bool isLoopback)
        {
            _roomId = roomId;
            _isLoopback = isLoopback;
        }

      

        protected override AppRTCEngine CreateEngine() => new AppRTCEngine(this);


        protected override void Connect(AppRTCEngine rtcEngine)
        {
            rtcEngine.Connect(new RoomConnectionParameters
            {
                RoomId = _roomId,
                IsLoopback = _isLoopback,
                RoomUrl = "https://appr.tc/"
            });
        }
    }
}
