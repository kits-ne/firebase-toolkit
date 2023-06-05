using System.Collections.Generic;

namespace FirebaseToolkit.Auth
{
    public enum LoginCredentialValidateMode
    {
        Latest,
        All
    }

    public partial class AuthConfig
    {
        public LoginCredentialValidateMode CredentialValidationOnLogin = LoginCredentialValidateMode.Latest;

        internal readonly Dictionary<string, ICredentialProvider> Providers =
            new Dictionary<string, ICredentialProvider>();

        public void AddCredentialProvider(ICredentialProvider provider)
        {
            Providers.Add(provider.ProviderId, provider);
        }
    }
}