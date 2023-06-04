using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

namespace FirebaseToolkit.Auth
{
    public class AuthController : IAuthController
    {
        private const string KeyLatestProviderId = "FBTK_LATEST_PROVIDER_ID";

        private readonly FirebaseAuth _auth;
        private readonly AuthConfig _config;

        private string LatestProviderId
        {
            get => PlayerPrefs.GetString(KeyLatestProviderId);
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    PlayerPrefs.DeleteKey(KeyLatestProviderId);
                }
                else
                {
                    PlayerPrefs.SetString(KeyLatestProviderId, value);
                }
            }
        }

        public AuthController(FirebaseAuth auth, AuthConfig config)
        {
            _auth = auth;
            _config = config;
        }

        public async UniTask<UserInfo> LoginAsync()
        {
            if (_auth.CurrentUser == null) return new UserInfo();

            var user = _auth.CurrentUser;
            var providerUsers = user.ProviderData.Select(_ => new SafeUserInfo(_)).ToArray();

            IUserInfo signedUser = null;
            if (providerUsers.Any())
            {
                var latestProviderId = LatestProviderId;
                signedUser = providerUsers.FirstOrDefault(_ => _.ProviderId == latestProviderId) ??
                             providerUsers.FirstOrDefault();
            }
            else
            {
                return new UserInfo(user);
            }

            if (signedUser == null || !_config.Providers.TryGetValue(signedUser.ProviderId, out var provider))
            {
                _auth.SignOut();
                throw new LoginFailedException(LoginFailReason.NotSupportProvider);
            }

            var isValid = await provider.Validate(signedUser);
            if (!isValid)
            {
                _auth.SignOut();
                throw new LoginFailedException(LoginFailReason.InvalidCredential);
            }

            return new UserInfo(user);
        }

        public async UniTask<UserInfo> SignInAsync(string providerId)
        {
            if (!_config.Providers.TryGetValue(providerId, out var provider))
            {
                throw new SignInFailedException(SignInFailReason.NotSupportProvider, providerId);
            }

            var credential = await provider.SignIn();
            var user = await _auth.SignInWithCredentialAsync(credential);

            LatestProviderId = providerId;
            return new UserInfo(user);
        }

        public async UniTask<bool> SignOutAsync(string providerId)
        {
            AuthResult result = null;
            try
            {
                result = await _auth.CurrentUser.UnlinkAsync(providerId);
            }
            catch (Exception e)
            {
                return false;
            }

            if (LatestProviderId == result.Credential.Provider)
            {
                LatestProviderId = null;
            }

            return true;
        }

        public bool IsSupportCredential(string providerId)
        {
            return _config.Providers.ContainsKey(providerId);
        }

        public bool IsConnectedProvider(string providerId)
        {
            if (_auth.CurrentUser == null) return false;

            foreach (var userInfo in _auth.CurrentUser.ProviderData)
            {
                if (userInfo.ProviderId == providerId)
                {
                    return true;
                }
            }

            return false;
        }

        public string[] GetConnectedProviders()
        {
            if (_auth.CurrentUser == null) return Array.Empty<string>();
            return _auth.CurrentUser.ProviderData.Select(_ => _.ProviderId).ToArray();
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