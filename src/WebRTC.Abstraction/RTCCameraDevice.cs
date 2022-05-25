using System;
using System.Linq;

namespace WebRTC.Abstraction
{
    public class RTCCameraDevice : IEquatable<RTCCameraDevice>
    {
        public RTCCameraDevice(string deviceId, bool isFront) : this(deviceId, isFront,
            GetDescription(deviceId, isFront))
        {
        }

        public RTCCameraDevice(string deviceId, bool isFront, string description)
        {
            DeviceId = deviceId;
            IsFront = isFront;
            Description = description;
        }

        public string DeviceId { get; }

        public bool IsFront { get; }

        public string Description { get; }

        public static RTCCameraDevice[] SupportedDevices { get; internal set; } = Array.Empty<RTCCameraDevice>();

        public static RTCCameraDevice GetFacingCameraOrDefault(bool isFront) =>
            SupportedDevices.FirstOrDefault(i => i.IsFront == isFront) ?? SupportedDevices.FirstOrDefault();

        private static string GetDescription(string deviceId, bool isFront)
        {
            return $"{(isFront ? "Front" : "Back")} {deviceId}";
        }

        public bool Equals(RTCCameraDevice? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DeviceId == other.DeviceId;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((RTCCameraDevice) obj);
        }

        public override int GetHashCode()
        {
            var hashCode = (DeviceId != null ? DeviceId.GetHashCode() : 0);
            return hashCode;
        }
    }
}