using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace FirebaseToolkit.Auth
{
    public partial class AuthConfig
    {
        public UserInfo EditorUser = new UserInfo();
    }

    public class EditorAuthController : IAuthController
    {
        private readonly HashSet<string> _providers;
        private readonly AuthConfig _config;

        private readonly HashSet<string> _signedProviders = new HashSet<string>();

        public EditorAuthController(AuthConfig config)
        {
            _config = config;
            _providers = new HashSet<string>(_config.Providers.Keys);
        }

        public UniTask<UserInfo> LoginAsync() => UniTask.FromResult(new UserInfo());

        public UniTask<UserInfo> SignInAsync(string providerId)
        {
            if (!_providers.Contains(providerId))
            {
                throw new LoginFailedException(LoginFailReason.NotSupportProvider);
            }

            var user = _config.EditorUser.IsValid()
                ? _config.EditorUser
                : new UserInfo(
                    "unity-editor",
                    "unity@editor.com"
                );

            _signedProviders.Add(providerId);
            return UniTask.FromResult(user);
        }

        public UniTask<string> LinkAsync(string providerId)
        {
            _signedProviders.Add(providerId);
            return UniTask.FromResult(providerId);
        }

        public UniTask<bool> SignOutAsync(string providerId)
        {
            _signedProviders.Remove(providerId);
            return UniTask.FromResult(true);
        }

        public bool IsSupportCredential(string providerId) => _providers.Contains(providerId);

        public bool IsConnectedProvider(string providerId) => _signedProviders.Contains(providerId);

        public string[] GetConnectedProviders() => _signedProviders.ToArray();
    }
}