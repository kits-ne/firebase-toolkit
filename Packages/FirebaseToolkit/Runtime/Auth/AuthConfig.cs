using System.Collections.Generic;

namespace FirebaseToolkit.Auth
{
    public partial class AuthConfig
    {

        internal readonly Dictionary<string, ICredentialProvider> Providers =
            new Dictionary<string, ICredentialProvider>();

        public void AddCredentialProvider(ICredentialProvider provider)
        {
            Providers.Add(provider.ProviderId, provider);
        }
    }
}