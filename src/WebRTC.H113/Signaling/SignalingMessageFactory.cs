using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebRTC.Abstraction;
using WebRTC.H113.Signaling.Models;

namespace WebRTC.H113.Signaling
{
    public static class SignalingMessageFactory
    {
        private static readonly JsonSerializerSettings _settings;

        static SignalingMessageFactory()
        {
            _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            _settings.Converters.Add(new StringEnumConverter());
        }

       
        
        public static SignalingMessage FromJson(string json)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            if (values.ContainsKey("type"))
            {
                switch (values["type"].ToString())
                {
                    case MessageTypesConstants.Registered:
                        return JsonConvert.DeserializeObject<RegisteredMessage>(json, _settings);
                    case MessageTypesConstants.SendOffer:
                        break;
                    case MessageTypesConstants.StopVideo:
                        return JsonConvert.DeserializeObject<StopVideoMessage>(json, _settings);
                    case MessageTypesConstants.ReceivedAnswer:
                        return GetAnswerSessionDescription(values["answer"].ToString());
                    case MessageTypesConstants.ReceiveCandidate:
                        return JsonConvert.DeserializeObject<IceCandidateMessage>(json, _settings);
                    case MessageTypesConstants.Reconnecting:
                        return JsonConvert.DeserializeObject<ReconnectingMessage>(json, _settings);
                    case MessageTypesConstants.CloseConnection:
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