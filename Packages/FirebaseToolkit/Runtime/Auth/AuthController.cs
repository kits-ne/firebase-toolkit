using System;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

namespace FirebaseToolkit.Auth
{
    public class AuthController
    {
        private const string KeyLatestProviderId = "FBTK_LATEST_PROVIDER_ID";

        private readonly AuthConfig _config;

        public AuthController(AuthConfig config)
        {
            _config = config;
        }

        public async Task<UserInfo> Login(FirebaseAuth auth)
        {
            if (_config.EditorUser.IsValid())
            {
                return _config.EditorUser;
            }

            if (auth.CurrentUser == null) return new UserInfo();

            var user = auth.CurrentUser;
            var providerUsers = user.ProviderData.Select(_ => new SafeUserInfo(_)).ToArray();

            IUserInfo signedUser = null;
            if (providerUsers.Any())
            {
                var latestProviderId = PlayerPrefs.GetString(KeyLatestProviderId);
                signedUser = providerUsers.FirstOrDefault(_ => _.ProviderId == latestProviderId) ??
                             providerUsers.FirstOrDefault();
            }

            if (signedUser == null || !_config.Providers.TryGetValue(signedUser.ProviderId, out var provider))
            {
                auth.SignOut();
                throw new LoginFailedException(LoginFailReason.NotSupportProvider);
            }

            var isValid = await provider.Validate(signedUser);
            if (!isValid)
            {
                auth.SignOut();
                throw new LoginFailedException(LoginFailReason.InvalidCredential);
            }

            return new UserInfo(user);
        }

        public async Task<UserInfo> SignIn(FirebaseAuth auth, string providerId)
        {
            if (!_config.Providers.TryGetValue(providerId, out var provider))
            {
                throw new SignInFailedException(SignInFailReason.NotSupportProvider, providerId);
            }

            var credential = await provider.SignIn();
            var user = await auth.SignInWithCredentialAsync(credential);

            PlayerPrefs.SetString(KeyLatestProviderId, providerId);
            return new UserInfo(user);
        }

        public bool IsSupportCredential(string providerId)
        {
            return _config.Providers.ContainsKey(providerId);
        }
    }

    internal class SafeUserInfo : IUserInfo
    {
        public string DisplayName { get; }
        public string Email { get; }
        public Uri PhotoUrl { get; }
        public string ProviderId { get; }
        public string UserId { get; }

        public SafeUserInfo(IUserInfo userInfo)
        {
            DisplayName = userInfo.DisplayName;
            Email = userInfo.Email;
            PhotoUrl = userInfo.PhotoUrl;
            ProviderId = userInfo.ProviderId;
            UserId = userInfo.UserId;
        }
    }
}