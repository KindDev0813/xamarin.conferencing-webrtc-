using System;
using System.Linq;
using CoreFoundation;
using Foundation;
using WebRTC.Abstraction;
using WebRTC.iOS.Extensions;

namespace WebRTC.iOS
{
    internal class PeerConnectionNative : NativeObjectBase, IPeerConnection
    {
        private readonly RTCPeerConnection _peerConnection;

        public PeerConnectionNative(RTCPeerConnection peerConnection, IPeerConnectionFactory factory)
        {
            _peerConnection = peerConnection;
            PeerConnectionFactory = factory;
        }

        public IPeerConnectionFactory PeerConnectionFactory { get; }
        public SessionDescription LocalDescription => _peerConnection.LocalDescription.ToNet();
        public SessionDescription RemoteDescription => _peerConnection.RemoteDescription.ToNet();
        public SignalingState SignalingState => _peerConnection.SignalingState.ToNet();
        public IceConnectionState IceConnectionState => _peerConnection.IceConnectionState.ToNet();
        public PeerConnectionState PeerConnectionState => _peerConnection.ConnectionState.ToNet();
        public IceGatheringState IceGatheringState => _peerConnection.IceGatheringState.ToNet();

        public IRtpSender[] Senders { get; }
        public IRtpReceiver[] Receivers { get; }
        public IRtpTransceiver[] Transceivers { get; }
        public Abstraction.RTCConfiguration Configuration { get; }

        public bool SetConfiguration(Abstraction.RTCConfiguration configuration)
        {
            return _peerConnection.SetConfiguration(configuration.ToNative());
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
            throw new NotImplementedException();
        }

        public bool RemoveTrack(IRtpSender sender)
        {
            throw new NotImplementedException();
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

        public void CreateOffer(MediaConstraints constraints, SdpCompletionHandler completionHandler)
        {
            _peerConnection.OfferForConstraints(constraints.ToNative(),
                (sdp, err) =>
                {
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        completionHandler?.Invoke(sdp.ToNet(), new Exception(err.LocalizedDescription));
                    });
                });
        }

        public void CreateAnswer(MediaConstraints constraints, SdpCompletionHandler completionHandler)
        {
            _peerConnection.AnswerForConstraints(constraints.ToNative(),
                (sdp, err) =>
                {
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        completionHandler?.Invoke(sdp.ToNet(), new Exception(err.LocalizedDescription));
                    });
                });
        }

        public void SetLocalDescription(SessionDescription sdp, Action<Exception> completionHandler)
        {
            _peerConnection.SetLocalDescription(sdp.ToNative(),
                (err) =>
                {
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                        completionHandler?.Invoke(new Exception(err.LocalizedDescription)));
                });
        }

        public void SetRemoteDescription(SessionDescription sdp, Action<Exception> completionHandler)
        {
            _peerConnection.SetRemoteDescription(sdp.ToNative(),
                (err) =>
                {
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                        completionHandler?.Invoke(new Exception(err.LocalizedDescription)));
                });
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
    }
}