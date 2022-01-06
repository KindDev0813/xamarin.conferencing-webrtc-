using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using WebRTC.Abstraction;
using WebRTC.Abstraction.Extensions;
using WebRTC.iOS.Extensions;
using WebRTC.iOS.Binding;


namespace WebRTC.iOS
{
    internal class PeerConnectionNative : NativeObjectBase, IPeerConnection
    {
        private readonly RTCPeerConnection _peerConnection;
        private readonly List<object> _csharpObjects = new();

        // ReSharper disable once NotAccessedField.Local
        // C# will dispose this object...
        private IRTCPeerConnectionDelegate _peerConnectionDelegate;

        public PeerConnectionNative(RTCPeerConnection peerConnection, Abstraction.RTCConfiguration configuration,
            IPeerConnectionFactory factory, IRTCPeerConnectionDelegate peerConnectionDelegate) : base(peerConnection)
        {
            _peerConnection = peerConnection;
            _peerConnectionDelegate = peerConnectionDelegate;
            Configuration = configuration;
            PeerConnectionFactory = factory;
        }

        public IPeerConnectionFactory PeerConnectionFactory { get; }
        public SessionDescription LocalDescription => _peerConnection.LocalDescription?.ToNet();
        public SessionDescription RemoteDescription => _peerConnection.RemoteDescription?.ToNet();
        public SignalingState SignalingState => _peerConnection.SignalingState.ToNet();
        public IceConnectionState IceConnectionState => _peerConnection.IceConnectionState.ToNet();
        public PeerConnectionState PeerConnectionState => _peerConnection.ConnectionState.ToNet();
        public IceGatheringState IceGatheringState => _peerConnection.IceGatheringState.ToNet();

        public IRtpSender[] Senders =>
            _peerConnection.Senders.Select(s => new RtpSenderNative(s)).Cast<IRtpSender>().ToArray();

        public IRtpReceiver[] Receivers =>
            _peerConnection.Receivers.Select(r => new RtpReceiverNative(r)).Cast<IRtpReceiver>().ToArray();


        public IRtpTransceiver[] Transceivers
        {
            get
            {
                if (Configuration.SdpSemantics != SdpSemantics.UnifiedPlan)
                    throw new InvalidOperationException(
                        "GetTransceivers is only supported with Unified Plan SdpSemantics.");
                return _peerConnection.Transceivers.Select(t => new RtpTransceiverNative(t)).Cast<IRtpTransceiver>()
                    .ToArray();
            }
        }

        public Abstraction.RTCConfiguration Configuration { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            _peerConnectionDelegate = null;
        }

        public bool SetConfiguration(Abstraction.RTCConfiguration configuration)
        {
            var result = _peerConnection.SetConfiguration(configuration.ToNative());
            if (result)
                Configuration = configuration;
            return result;
        }

        public void Close()
        {
            _peerConnection.Close();
        }

        public void AddIceCandidate(IceCandidate candidate)
        {
            _peerConnection.AddIceCandidate(candidate.ToNative());
        }

        public void RemoveIceCandidates(IceCandidate[] candidates)
        {
            _peerConnection.RemoveIceCandidates(candidates.ToNative().ToArray());
        }

        public void AddStream(IMediaStream stream)
        {
            _peerConnection.AddStream(stream.ToNative<RTCMediaStream>());
        }

        public void RemoveStream(IMediaStream stream)
        {
            _peerConnection.RemoveStream(stream.ToNative<RTCMediaStream>());
        }

        public IRtpSender AddTrack(IMediaStreamTrack track, string[] streamIds)
        {
            var rtpSender = _peerConnection.AddTrack(track.ToNative<RTCMediaStreamTrack>(), streamIds);
            return rtpSender == null ? null : new RtpSenderNative(rtpSender);
        }

        public bool RemoveTrack(IRtpSender sender)
        {
            return _peerConnection.RemoveTrack(sender.ToNative<IRTCRtpSender>());
        }

        public IRtpTransceiver AddTransceiverWithTrack(IMediaStreamTrack track)
        {
            var rtpTransceiver = _peerConnection.AddTransceiverWithTrack(track.ToNative<RTCMediaStreamTrack>());
            return rtpTransceiver == null ? null : new RtpTransceiverNative(rtpTransceiver);
        }

        public IRtpTransceiver AddTransceiverWithTrack(IMediaStreamTrack track, IRtpTransceiverInit init)
        {
            throw new NotImplementedException();
        }

        public IRtpTransceiver AddTransceiverOfType(RtpMediaType mediaType)
        {
            throw new NotImplementedException();
        }

        public IRtpTransceiver AddTransceiverOfType(RtpMediaType mediaType, IRtpTransceiverInit init)
        {
            throw new NotImplementedException();
        }

        public void CreateOffer(MediaConstraints constraints, ISdpObserver observer)
        {
            var sdpCallbacksHelper = new SdpCallbackHelper(observer, this);

            _peerConnection.OfferForConstraints(constraints.ToNative(), sdpCallbacksHelper.CreateSdp);
        }

        public void CreateAnswer(MediaConstraints constraints, ISdpObserver observer)
        {
            var sdpCallbacksHelper = new SdpCallbackHelper(observer, this);

            _peerConnection.AnswerForConstraints(constraints.ToNative(), sdpCallbacksHelper.CreateSdp);
        }

        public void SetLocalDescription(SessionDescription sdp, ISdpObserver observer)
        {
            var sdpCallbacksHelper = new SdpCallbackHelper(observer, this);

            _peerConnection.SetLocalDescription(sdp.ToNative(), sdpCallbacksHelper.SetSdp);
        }

        public void SetRemoteDescription(SessionDescription sdp, ISdpObserver observer)
        {
            var sdpCallbacksHelper = new SdpCallbackHelper(observer, this);

            _peerConnection.SetRemoteDescription(sdp.ToNative(), sdpCallbacksHelper.SetSdp);
        }

        public IDataChannel CreateDataChannel(string label, DataChannelConfiguration dataChannelConfiguration)
        {
            var dataChannel = _peerConnection.DataChannelForLabel(label, dataChannelConfiguration.ToNative());
            return dataChannel == null ? null : new DataChannelNative(dataChannel);
        }

        public bool SetBitrate(int min, int current, int max)
        {
            return _peerConnection.SetBweMinBitrateBps(new NSNumber(min), new NSNumber(current), new NSNumber(max));
        }

        public void StopRtcEventLog()
        {
            _peerConnection.StopRtcEventLog();
        }

        public bool StartRtcEventLog(string file, int fileSizeLimitBytes)
        {
            return _peerConnection.StartRtcEventLogWithFilePath(file, fileSizeLimitBytes);
        }

        private class SdpCallbackHelper
        {
            private readonly ISdpObserver _observer;
            private readonly PeerConnectionNative _peerConnectionNative;

            public SdpCallbackHelper(ISdpObserver observer, PeerConnectionNative peerConnectionNative)
            {
                _observer = observer;
                _peerConnectionNative = peerConnectionNative;
                _peerConnectionNative._csharpObjects.Add(this);
            }

            public void SetSdp(NSError error)
            {
                if (error != null)
                    _observer?.OnSetFailure(error.LocalizedDescription);
                else
                    _observer?.OnSetSuccess();
                Clear();
            }

            public void CreateSdp(RTCSessionDescription sdp, NSError error)
            {
                if (error != null)
                {
                    _observer?.OnCreateFailure(error.LocalizedDescription);
                }
                else
                {
                    _observer?.OnCreateSuccess(sdp.ToNet());
                }

                Clear();
            }

            private void Clear()
            {
                _peerConnectionNative._csharpObjects.Remove(this);
            }
        }
    }
}