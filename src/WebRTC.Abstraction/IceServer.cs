namespace WebRTC.Abstraction
{
    public class IceServer
    {
        public IceServer(string uri, string username = "", string password = "",
            TlsCertPolicy tlsCertPolicy = TlsCertPolicy.Secure) : this(new[] {uri}, username, password, tlsCertPolicy)
        {
        }

        public IceServer(string[] urls, string username, string password,
            TlsCertPolicy tlsCertPolicy = TlsCertPolicy.Secure)
        {
            Urls = urls;
            Username = username;
            Password = password;
            TlsCertPolicy = tlsCertPolicy;
        }

        public string[] Urls { get; }
        public string Username { get; }
        public string Password { get; }
        public TlsCertPolicy TlsCertPolicy { get; }

        public override string ToString()
        {
            return $"{Urls} [{Username}:{Password}] [{TlsCertPolicy}]";
        }
    }
}