namespace WebRTC.Abstraction
{
    public class IceServer
    {
        public IceServer(string uri) : this(uri, "", "")
        {
        }

        public IceServer(string uri, string username, string password) : this(uri, username, password,
            TlsCertPolicy.Secure)
        {
        }

        public IceServer(string uri, string username, string password, TlsCertPolicy tlsCertPolicy)
        {
            Urls = new[] {uri};
            Username = username;
            Password = password;
        }

        public string[] Urls { get; }
        public string Username { get; }
        public string Password { get; }
        public TlsCertPolicy TlsCertPolicy { get; }
    }
}