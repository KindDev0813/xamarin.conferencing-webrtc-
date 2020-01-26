using System;
using System.IO;
using WebRTC.Abstraction;

namespace WebRTC.AppRTC
{
    public class RtcEventLog
    {
        private enum RtcEventLogState
        {
            Inactive,
            Started,
            Stopped,
        }

        
        private const string TAG = nameof(RtcEventLog);

        private static int OutputFileMaxBytes = 10_000_000;

        private readonly IPeerConnection _peerConnection;
        private readonly ILogger _logger;
        private readonly string _file;

        private RtcEventLogState _state;

      
        
        public RtcEventLog(IPeerConnection peerConnection,string file, ILogger logger = null)
        {
            _peerConnection = peerConnection ?? throw new ArgumentNullException(nameof(peerConnection), "The peer connection is null.");
            _logger = logger ?? new ConsoleLogger();
            _file = file;
        }

        public void Start()
        {
            if (_state == RtcEventLogState.Started)
            {
                _logger.Debug(TAG,"RtcEventLog has already started.");
                return;
            }

            var success = _peerConnection.StartRtcEventLog(_file, OutputFileMaxBytes);
            if (!success)
            {
                _logger.Error(TAG,"Failed to start RTC event log.");
                return;
            }

            _state = RtcEventLogState.Started;
            _logger.Debug(TAG,"RtcEventLog started.");
        }

        public void Stop()
        {
            if (_state != RtcEventLogState.Started)
            {
                _logger.Error(TAG,"RtcEventLog was not started.");
                return;
            }
            _peerConnection.StopRtcEventLog();
            _state = RtcEventLogState.Stopped;
            _logger.Debug(TAG,"RtcEventLog stopped.");
        }
    }
}