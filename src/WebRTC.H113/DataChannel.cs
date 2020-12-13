using System;
using System.Text;
using WebRTC.Abstraction;

namespace WebRTC.H113
{
    public class DataChannel : IDisposable
    {
        private const string SwitchCameraCommand = "flip cam";
        private const string ActivateAppCommand = "notify";

        private readonly IDataChannelEvents _dataChannelEvents;
        private readonly IDataChannel _dataChannel;

        public DataChannel(IDataChannelEvents dataChannelEvents,  IPeerConnection peerConnection)
        {
            _dataChannelEvents = dataChannelEvents;
            _dataChannel = peerConnection.CreateDataChannel("sendChannel", new DataChannelConfiguration());
            if (_dataChannel != null)
                _dataChannel.OnMessage += DataChannelOnOnMessage;
        }
        
        public void Dispose()
        {
            if (_dataChannel != null)
                _dataChannel.OnMessage -= DataChannelOnOnMessage;
            _dataChannel?.Dispose();
        }
        
        private void DataChannelOnOnMessage(object sender, DataBuffer e)
        {
            var command = Encoding.UTF8.GetString(e.Data);
            switch (command)
            {
                case SwitchCameraCommand:
                    _dataChannelEvents.SwitchCamera();
                    break;
                case ActivateAppCommand:
                    _dataChannelEvents.ShowNotification(1, "Title", "Message body");
                    break;
            }
        }
        
    }
}