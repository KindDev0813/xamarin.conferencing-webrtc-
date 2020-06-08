using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebRTC.Abstraction;
using WebRTC.H113.Signaling.Models;

namespace WebRTC.H113.Signaling
{
    public static class SignalingMessageFactory
    {
        private static JsonSerializerSettings _settings;

        static SignalingMessageFactory()
        {
            _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            _settings.Converters.Add(new StringEnumConverter());
        }

        public const string Register = "new-app-connection";
        public const string Registered = "app-start-video";
        public const string SendOffer = "amk-send-offer";
        public const string ReceivedAnswer = "receive-answer";
        public const string ReceiveCandidate = "receive-candidate";
        public const string SendCandidate = "amk-send-candidate";
        public const string Reconnecting = "app-connection-id";
        public const string UpdateInfo = "app-update-info";
        public const string DoReconnect = "app-reconnecting-ws";
        public const string StopVideo = "app-amk-stop-video";
        public const string CloseConnection = "app-close-connection";
        
        public static SignalingMessage FromJson(string json)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            if (values.ContainsKey("type"))
            {
                switch (values["type"].ToString())
                {
                    case Registered:
                        return JsonConvert.DeserializeObject<RegisteredMessage>(json, _settings);
                    case SendOffer:
                        break;
                    case StopVideo:
                        return JsonConvert.DeserializeObject<StopVideoMessage>(json, _settings);
                    case ReceivedAnswer:
                        return GetAnswerSessionDescription(values["answer"].ToString());
                    case ReceiveCandidate:
                        return JsonConvert.DeserializeObject<IceCandidateMessage>(json, _settings);
                    case Reconnecting:
                        return JsonConvert.DeserializeObject<ReconnectingMessage>(json, _settings);
                    case CloseConnection:
                        return JsonConvert.DeserializeObject<CloseConnectionMessage>(json, _settings);
                }
            }

            return null;
        }
        
        public static string ToJson(SignalingMessage signalingMessage) => JsonConvert.SerializeObject(signalingMessage, _settings);

        private static SessionDescriptionMessage GetAnswerSessionDescription(string json)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json, _settings);

            return new SessionDescriptionMessage
            {
                MessageType = SignalingMessageType.ReceivedAnswer,
                Description = new SessionDescription(SdpType.Answer, values?["sdp"])
            };
        }
    }
}