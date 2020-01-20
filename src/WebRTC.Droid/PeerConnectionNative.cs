using System;
using System.Collections.Generic;
using System.Linq;
using Org.Webrtc;
using WebRTC.Abstraction;
using WebRTC.Droid.Extensions;
using IceCandidate = WebRTC.Abstraction.IceCandidate;
using MediaConstraints = WebRTC.Abstraction.MediaConstraints;
using SessionDescription = WebRTC.Abstraction.SessionDescription;

namespace WebRTC.Droid
{
    public class PeerConnectionNative : IPeerConnection
    {
        private readonly Org.Webrtc.PeerConnection _peerConnection;
        
        private readonly List<RtpSenderNative> _senders = new List<RtpSenderNative>();
        private readonly List<RtpReceiverNative> _receivers = new List<RtpReceiverNative>();

        public PeerConnectionNative(Org.Webrtc.PeerConnection peerConnection)
        {
            NativeObject = _peerConnection = peerConnection;
        }
        
        public object NativeObject { get; }
        
        //public IMediaStream[] LocalStreams { get; }
        public SessionDescription LocalDescription => _peerConnection.LocalDescription.ToNet();
        public SessionDescription RemoteDescription => _peerConnection.RemoteDescription.ToNet();
        public SignalingState SignalingState => _peerConnection.InvokeSignalingState().ToNet();
        public IceConnectionState IceConnectionState => _peerConnection.InvokeIceConnectionState().ToNet();
        public PeerConnectionState PeerConnectionState => _peerConnection.inv
        public IceGatheringState IceGatheringState => _peerConnection.InvokeIceGatheringState().ToNet();
        public IRtpSender[] Senders { get; }
        public IRtpReceiver[] Receivers { get; }
        public IRtpTransceiver[] Transceivers { get; }

        public RTCConfiguration Configuration { get; private set; }

        public bool SetConfiguration(RTCConfiguration configuration)
        {
            Configuration = configuration;
            return _peerConnection.SetConfiguration(configuration.ToNative());
        }

        public void Dispose()
        {
            _peerConnection.Dispose();
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
            _peerConnection.RemoveIceCandidates(candidates.Select(c => c.ToNative()).ToArray());
        }

        public void AddStream(IMediaStream stream)
        {
            _peerConnection.AddStream(((MediaStreamNative) stream).NativeMediaStream);
        }

        public void RemoveStream(IMediaStream stream)
        {
            _peerConnection.RemoveStream(((MediaStreamNative) stream).NativeMediaStream);
        }

        public IRtpSender AddTrack(IMediaStreamTrack track, string[] streamIds)
        {
            var rtpSender = new RtpSenderNative(_peerConnection.AddTrack(track.ToNative(), streamIds));
            return rtpSender;
        }

        public bool RemoveTrack(IRtpSender sender)
        {
            return _peerConnection.RemoveTrack(sender.ToNative<RtpSender>());
        }

        public IRtpTransceiver AddTransceiverWithTrack(IMediaStreamTrack track)
        {
            throw new NotImplementedException();
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

        public void OfferForConstraints(MediaConstraints constraints,
            Action<SessionDescription, Exception> completionHandler)
        {
            throw new NotImplementedException();
        }

        public void AnswerForConstraints(MediaConstraints constraints,
            Action<SessionDescription, Exception> completionHandler)
        {
            throw new NotImplementedException();
        }

        public void SetLocalDescription(SessionDescription sdp, Action<Exception> completionHandler)
        {
            throw new NotImplementedException();
        }

        public void SetRemoteDescription(SessionDescription sdp, Action<Exception> completionHandler)
        {
            throw new NotImplementedException();
        }

        public bool SetBitrate(int min, int current, int max)
        {
            throw new NotImplementedException();
        }

        public bool StartRtcEventLogWithFilePath(string filePath, long maxSizeInBytes)
        {
            throw new NotImplementedException();
        }

        public void StopRtcEventLog()
        {
            throw new NotImplementedException();
        }

    }
}