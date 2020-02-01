namespace WebRTC.H113
{
    public static class H113Constants
    {
        public static string Token { get; set; }

        public const string Phone = "98056391";

        public const string ApiUrl = "https://video.h113.no/api/";

        public const string LoginUrl = ApiUrl + "login/mobile";

        public const string CodeUrl = ApiUrl + "login/code";

        public const string WssUrl = "wss://video.h113.no/ws";
    }
}