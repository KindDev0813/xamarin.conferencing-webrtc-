using System;
using WebRTC.AppRTC;

namespace WebRTC.iOS.Demo
{
    public class AppRTCCallViewController : CallViewControllerBase<RoomConnectionParameters,SignalingParameters, AppRTCController>
    {
        private readonly string _roomId;
        private readonly bool _isLoopback;

        public AppRTCCallViewController(string roomId,bool isLoopback)
        {
            _roomId = roomId;
            _isLoopback = isLoopback;
        }

      

        protected override AppRTCController CreateController() => new AppRTCController(this);


        protected override void Connect(AppRTCController rtcController)
        {
            rtcController.Connect(new RoomConnectionParameters
            {
                RoomId = _roomId,
                IsLoopback = _isLoopback,
                RoomUrl = "https://appr.tc/"
            });
        }
    }
}
