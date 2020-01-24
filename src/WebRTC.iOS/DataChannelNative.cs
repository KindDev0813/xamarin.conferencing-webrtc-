using System;
using Foundation;
using WebRTC.Abstraction;
using WebRTC.iOS.Extensions;

namespace WebRTC.iOS
{
    internal class DataChannelNative : NSObject,IDataChannel,IRTCDataChannelDelegate
    {
        private readonly RTCDataChannel _dataChannel;

        public DataChannelNative(RTCDataChannel dataChannel)
        {
            _dataChannel = dataChannel;
        }

        public event EventHandler OnStateChange;
        public event EventHandler<DataBuffer> OnMessage;
        public event EventHandler<long> OnBufferedAmountChange;
        public int Id => _dataChannel.ChannelId;
        public string Label => _dataChannel.Label;
        public DataChannelState State => _dataChannel.ReadyState.ToNet();
        public long BufferedAmount => (long)_dataChannel.BufferedAmount;
        
        public void Close()
        {
            _dataChannel.Close();
        }

        public bool Send(DataBuffer dataBuffer)
        {
            return _dataChannel.SendData(new RTCDataBuffer(NSData.FromArray(dataBuffer.Data), dataBuffer.IsBinary));
        }

        public void DataChannelDidChangeState(RTCDataChannel dataChannel)
        {
            OnStateChange?.Invoke(this,EventArgs.Empty);
        }

        public void DidReceiveMessageWithBuffer(RTCDataChannel dataChannel, RTCDataBuffer buffer)
        {
            var netBuffer = new DataBuffer(buffer.Data.ToArray(),buffer.IsBinary);
            OnMessage?.Invoke(this,netBuffer);
        }
    }
}