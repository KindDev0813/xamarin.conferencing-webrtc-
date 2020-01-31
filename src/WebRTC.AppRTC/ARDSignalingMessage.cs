using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WebRTC.Abstraction;

namespace WebRTC.AppRTC
{
    public enum ARDSignalingMessageType
    {
        Candidate,
        CandidateRemoval,
        Offer,
        Answer,
        Bye,
    }

    public class ARDSignalingMessage
    {
        protected const string CandidateType = "candidate";
        protected const string CandidateRemovalType = "remove-candidates";
        protected const string OfferType = "offer";
        protected const string AnswerType = "answer";
        protected const string ByeType = "bye";

        public static ARDSignalingMessage MessageFromJSONString(string json)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            ARDSignalingMessage message = new ARDSignalingMessage();

            if (values.ContainsKey("type"))
            {
                var type = values["type"] ?? "";
                switch (type)
                {
                    case CandidateType:
                        int.TryParse(values["label"], out int label);
                        var candidate = new IceCandidate(values["candidate"], values["id"], label);
                        message = new ARDICECandidateMessage(candidate);
                        break;
                    case CandidateRemovalType:
                        
                        break;
                    case OfferType:
                        var description = new SessionDescription(SdpType.Offer, values["sdp"]);
                        message = new ARDSessionDescriptionMessage(description);
                        break;
                    case AnswerType:
                        description = new SessionDescription(SdpType.Answer, values["sdp"]);
                        message = new ARDSessionDescriptionMessage(description);
                        break;
                    case ByeType:
                        message = new ARDByeMessage();
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine($"ARDSignalingMessage unexpected type: {type}");
                        break;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"ARDSignalingMessage invalid json: {json}");
            }

            return message;
        }
        
        public static string CreateJson(IceCandidate iceCandidate) => new ARDICECandidateMessage(iceCandidate).JsonData;
        public static string CreateJson(SessionDescription sessionDescription) => new ARDSessionDescriptionMessage(sessionDescription).JsonData;
        public static string CreateJson(IceCandidate[] removalCandidates) => new ARDICECandidateRemovalMessage(removalCandidates).JsonData;

        public static string CreateByeJson()=> new ARDByeMessage().JsonData;
        
        public ARDSignalingMessageType Type { get; set; }

        public virtual string JsonData { get; } = "{}";

        public override string ToString()
        {
            return JsonData;
        }

        public string ToStringPrettyPrinted()
        {
            var obj = new {type = Type.ToString(), message = JsonData.ToString()};
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
        


        protected static string ToJsonCandidate(IceCandidate iceCandidate)
        {
            return JsonConvert.SerializeObject(new
            {
                label = iceCandidate.SdpMLineIndex,
                id = iceCandidate.SdpMid,
                candidate = iceCandidate.Sdp
            });
        }

        protected static string ToJsonCandidates(IEnumerable<IceCandidate> iceCandidates)
        {
            return JsonConvert.SerializeObject(iceCandidates.Select(ToJsonCandidate));
        }

        protected static string GetTypeString(ARDSignalingMessageType type)
        {
            switch (type)
            {
                case ARDSignalingMessageType.Candidate:
                    return CandidateRemovalType;
                case ARDSignalingMessageType.CandidateRemoval:
                    return CandidateRemovalType;
                case ARDSignalingMessageType.Offer:
                    return OfferType;
                case ARDSignalingMessageType.Answer:
                    return AnswerType;
                case ARDSignalingMessageType.Bye:
                    return ByeType;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }

    public class ARDICECandidateMessage : ARDSignalingMessage
    {
        public ARDICECandidateMessage(IceCandidate candidate)
        {
            Type = ARDSignalingMessageType.Candidate;
            Candidate = candidate;
        }

        public IceCandidate Candidate { get; set; }

        public override string JsonData => JsonConvert.SerializeObject(new
        {
            type = GetTypeString(Type),
            label = Candidate.SdpMLineIndex,
            id = Candidate.SdpMid,
            candidate = Candidate.Sdp
        });
    }

    public class ARDICECandidateRemovalMessage : ARDSignalingMessage
    {
        public ARDICECandidateRemovalMessage(IceCandidate[] candidates)
        {
            Type = ARDSignalingMessageType.CandidateRemoval;
            Candidates = candidates;
        }

        public IceCandidate[] Candidates { get; set; }

        public override string JsonData => JsonConvert.SerializeObject(new
        {
            type = GetTypeString(Type),
            candidates = ToJsonCandidates(Candidates)
        });
    }

    public class ARDSessionDescriptionMessage : ARDSignalingMessage
    {
        public ARDSessionDescriptionMessage(SessionDescription description)
        {
            Description = description;
            switch (Description.Type)
            {
                case SdpType.Offer:
                    Type = ARDSignalingMessageType.Offer;
                    break;
                case SdpType.PrAnswer:
                    break;
                case SdpType.Answer:
                    Type = ARDSignalingMessageType.Answer;
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine($"ARDSessionDescriptionMessage unexpected type: {Type}");
                    break;
            }
        }

        public SessionDescription Description { get; set; }

        public override string JsonData => JsonConvert.SerializeObject(new
        {
            type = GetTypeString(Type),
            sdp = Description.Sdp
        });
    }

    public class ARDByeMessage : ARDSignalingMessage
    {
        public ARDByeMessage()
        {
            Type = ARDSignalingMessageType.Bye;
        }

        public override string JsonData => JsonConvert.SerializeObject(new {type = "bye"});
    }
}