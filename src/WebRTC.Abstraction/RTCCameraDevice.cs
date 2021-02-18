namespace WebRTC.Abstraction
{
    public class RTCCameraDevice
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

        public static RTCCameraDevice[] SupportedDevices { get; internal set; }

        private static string GetDescription(string deviceId, bool isFront)
        {
            return $"{(isFront ? "Front" : "Back")} {deviceId}";
        }
    }
}