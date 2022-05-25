using System.Text;

namespace WebRTC.Abstraction
{
    public record IceServer(string[] Urls, string Username, string Password,
        TlsCertPolicy TlsCertPolicy = TlsCertPolicy.Secure)
    {
        public IceServer(string uri, string username = "", string password = "",
            TlsCertPolicy tlsCertPolicy = TlsCertPolicy.Secure) : this(new[] {uri}, username, password, tlsCertPolicy)
        {
        }

        public string[] Urls { get; } = Urls;
        public string Username { get; } = Username;
        public string Password { get; } = Password;
        public TlsCertPolicy TlsCertPolicy { get; } = TlsCertPolicy;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            foreach (var url in Urls)
            {
                sb.Append(url).Append(", ");
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append("] ");
            if (!string.IsNullOrEmpty(Username))
                sb.Append("[").Append(Username).Append(":").Append("] ");
            sb.Append("[").Append(TlsCertPolicy).Append("]");
            return sb.ToString();
        }
    }
}