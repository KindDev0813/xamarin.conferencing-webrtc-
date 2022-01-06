using System.Collections.Generic;

namespace WebRTC.Abstraction
{
    public class MediaConstraints
    {
        public MediaConstraints() : this(new Dictionary<string, string>())
        {
        }

        public MediaConstraints(IDictionary<string, string> mandatory, IDictionary<string, string>? optional = null)
        {
            Mandatory = mandatory;
            Optional = optional ?? new Dictionary<string, string>();
        }

        public IDictionary<string, string> Mandatory { get; }
        public IDictionary<string, string> Optional { get; }
    }
}