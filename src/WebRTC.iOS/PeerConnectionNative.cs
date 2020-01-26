using System;
using System.Linq;
using CoreFoundation;
using Foundation;
using WebRTC.Abstraction;
using WebRTC.iOS.Extensions;
using WebRTC.iOS.Binding;


namespace WebRTC.iOS
{
    internal class PeerConnectionNative : NativeObjectBase, IPeerConnection
    {
        private readonly RTCPeerConnection _peerConnection;

        public PeerConnectionNative(RTCPeerConnection peerConnection,Abstraction.RTCConfiguration configuration, IPeerConnectionFactory factory)
        {
            _peerConnection = peerConnection;
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
                    throw new InvalidOperationException("GetTransceivers is only supported with Unified Plan SdpSemantics.");
                return _peerConnection.Transceivers.Select(t => new RtpTransceiverNative(t)).Cast<IRtpTransceiver>().ToArray();
            }
        }

        public Abstraction.RTCConfiguration Configuration { get; private set; }

        public bool SetConfiguration(Abstraction.RTCConfiguration configuration)
        {
            var result =  _peerConnection.SetConfiguration(configuration.ToNative());
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
            if (rtpSender == null)
                return null;
            return new RtpSenderNative(rtpSender);
        }

        public bool RemoveTrack(IRtpSender sender)
        {
            return _peerConnection.RemoveTrack(sender.ToNative<IRTCRtpSender>());
        }

        public IRtpTransceiver AddTransceiverWithTrack(IMediaStreamTrack track)
        {
            var rtpTransceiver = _peerConnection.AddTransceiverWithTrack(track.ToNative<RTCMediaStreamTrack>());
            if (rtpTransceiver == null)
                return null;
            return new RtpTransceiverNative(rtpTransceiver);
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

        public void CreateOffer(MediaConstraints constraints, SdpCompletionHandler completionHandler)
        {
            var sdpCallbacksHelper = new SdpCallbackHelper(completionHandler);

            _peerConnection.OfferForConstraints(constraints.ToNative(),sdpCallbacksHelper.CreateSdp);
        }

        public void CreateAnswer(MediaConstraints constraints, SdpCompletionHandler completionHandler)
        {
            var sdpCallbacksHelper = new SdpCallbackHelper(completionHandler);

            _peerConnection.OfferForConstraints(constraints.ToNative(), sdpCallbacksHelper.CreateSdp);
        }

        public void SetLocalDescription(SessionDescription sdp, Action<Exception> completionHandler)
        {
            var sdpCallbacksHelper = new SdpCallbackHelper((_,err)=>completionHandler?.Invoke(err));

            _peerConnection.SetLocalDescription(sdp.ToNative(), sdpCallbacksHelper.SetSdp);
          
        }

        public void SetRemoteDescription(SessionDescription sdp, Action<Exception> completionHandler)
        {
            var sdpCallbacksHelper = new SdpCallbackHelper((_, err) => completionHandler?.Invoke(err));

            _peerConnection.SetRemoteDescription(sdp.ToNative(), sdpCallbacksHelper.SetSdp);
        }

        public bool SetBitrate(int min, int current, int max)
        {
            return _peerConnection.SetBweMinBitrateBps(new NSNumber(min), new NSNumber(current), new NSNumber(max));
        }

        public bool StartRtcEventLogWithFilePath(string filePath, long maxSizeInBytes)
        {
            return _peerConnection.StartRtcEventLogWithFilePath(filePath, maxSizeInBytes);
        }

        public void StopRtcEventLog()
        {
            _peerConnection.StopRtcEventLog();
        }

        private class SdpCallbackHelper
        {
            private readonly SdpCompletionHandler _completionHandler;

            public SdpCallbackHelper(SdpCompletionHandler completionHandler)
            {
                _completionHandler = completionHandler;
            }

            public void SetSdp(NSError error)
            {
                DispatchQueue.MainQueue.DispatchAsync(() => _completionHandler?.Invoke(null, error == null ? null : new Exception(error.LocalizedDescription)));
            }

            public void CreateSdp(RTCSessionDescription sdp,NSError error)
            {

                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    if (error != null)
                    {
                        _completionHandler?.Invoke(null, new Exception(error.LocalizedDescription));
                        return;
                    }
                    _completionHandler?.Invoke(sdp.ToNet(), null);
                });
            }

        }
    }
}