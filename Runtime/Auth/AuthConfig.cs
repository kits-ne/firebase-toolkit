using System.Collections.Generic;

namespace FirebaseToolkit.Auth
{
    public class AuthConfig
    {
        public UserInfo EditorUser;

        internal readonly Dictionary<string, ICredentialProvider> Providers =
            new Dictionary<string, ICredentialProvider>();

        public void AddCredentialProvider(ICredentialProvider provider)
        {
            Providers.Add(provider.ProviderId, provider);
        }
    }
}