using System;

namespace WebRTC.Abstraction
{
    public interface IPeerConnection :INativeObject, IDisposable
    {
        //IMediaStream[] LocalStreams { get; }
        SessionDescription LocalDescription { get; }
        SessionDescription RemoteDescription { get; }
        
        SignalingState SignalingState { get; }
        
        IceConnectionState IceConnectionState { get; }
        
        PeerConnectionState PeerConnectionState { get; }
        
        IceGatheringState IceGatheringState { get; }
        
        IRtpSender[] Senders { get; }
        
        IRtpReceiver[] Receivers { get; }
        
        IRtpTransceiver[] Transceivers { get; }
        
        RTCConfiguration Configuration { get; }

        bool SetConfiguration(RTCConfiguration configuration);

        void Close();
        
        void AddIceCandidate(IceCandidate candidate);
        void RemoveIceCandidates(IceCandidate[] candidates);
        void AddStream(IMediaStream stream);
        void RemoveStream(IMediaStream stream);

        IRtpSender AddTrack(IMediaStreamTrack track, string[] streamIds);

        bool RemoveTrack(IRtpSender sender);
        
        IRtpTransceiver AddTransceiverWithTrack(IMediaStreamTrack track);
        IRtpTransceiver AddTransceiverWithTrack(IMediaStreamTrack track, IRtpTransceiverInit init);

        IRtpTransceiver AddTransceiverOfType(RtpMediaType mediaType);
        IRtpTransceiver AddTransceiverOfType(RtpMediaType mediaType, IRtpTransceiverInit init);
        
        void OfferForConstraints(MediaConstraints constraints,  Action<SessionDescription, Exception> completionHandler);
        void AnswerForConstraints(MediaConstraints constraints,  Action<SessionDescription, Exception> completionHandler);

        void SetLocalDescription(SessionDescription sdp,  Action<Exception> completionHandler);
        void SetRemoteDescription(SessionDescription sdp,  Action<Exception> completionHandler);

        bool SetBitrate(int min, int current, int max);
        bool StartRtcEventLogWithFilePath(string filePath, long maxSizeInBytes);
        void StopRtcEventLog();
    }

    public interface IRtpTransceiverInit
    {
    }
}