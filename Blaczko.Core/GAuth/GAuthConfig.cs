using Blaczko.Core.Configuration;

namespace Blaczko.Core.GAuth
{
    public class GAuthConfig : ConfigModel
    {
        [RequiredKey]
        public string ClientId { get; set; }
    }
}
