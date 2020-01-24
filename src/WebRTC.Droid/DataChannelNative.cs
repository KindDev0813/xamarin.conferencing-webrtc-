using System;
using System.Linq;
using Java.Nio;
using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;

namespace WebRTC.Droid
{
    internal class DataChannelNative : Java.Lang.Object, IDataChannel, DataChannel.IObserver
    {
        private readonly DataChannel _dataChannel;

        public DataChannelNative(DataChannel dataChannel)
        {
            _dataChannel = dataChannel;
            _dataChannel.RegisterObserver(this);
        }

        public event EventHandler OnStateChange;

        public event EventHandler<DataBuffer> OnMessage;

        public event EventHandler<long> OnBufferedAmountChange;

        public int Id => _dataChannel.Id();

        public string Label => _dataChannel.Label();

        public DataChannelState State => _dataChannel.InvokeState().ToNet();

        public long BufferedAmount => _dataChannel.BufferedAmount();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dataChannel.UnregisterObserver();
            }

            base.Dispose(disposing);
        }

        public void Close()
        {
            _dataChannel.Close();
        }

        public bool Send(DataBuffer dataBuffer)
        {
            return _dataChannel.Send(new DataChannel.Buffer(ByteBuffer.Wrap(dataBuffer.Data), dataBuffer.IsBinary));
        }

        void DataChannel.IObserver.OnBufferedAmountChange(long p0)
        {
            OnBufferedAmountChange?.Invoke(this, p0);
        }

        void DataChannel.IObserver.OnMessage(DataChannel.Buffer p0)
        {
            OnMessage?.Invoke(this, new DataBuffer(new FastJavaByteArray(p0.Data.Handle).ToArray(), p0.Binary));
        }

        void DataChannel.IObserver.OnStateChange()
        {
            OnStateChange?.Invoke(this, EventArgs.Empty);
        }
    }
}