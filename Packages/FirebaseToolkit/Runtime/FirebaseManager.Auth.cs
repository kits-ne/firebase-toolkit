#if FBTK_AUTH
using System;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using FirebaseToolkit.Auth;
using UnityEngine;

namespace FirebaseToolkit
{
    public partial class FirebaseConfig
    {
        public readonly AuthConfig Auth = new AuthConfig();
    }

    public static partial class FirebaseManager
    {
        private static IAuthController _authController;
        private static FirebaseAuth _auth;

        static partial void AppInitialized(FirebaseConfig config)
        {
            _auth = FirebaseAuth.DefaultInstance;
#if UNITY_EDITOR
            _authController = new EditorAuthController(config.Auth);
#else
            _authController = new AuthController(_auth, config.Auth);
#endif
        }

        public static UserInfo GetUser() => new UserInfo(_auth.CurrentUser);

        public static bool IsSupportCredential(string providerId) =>
            _authController?.IsSupportCredential(providerId) ?? false;

        public static bool IsConnectedProvider(string providerId) =>
            _authController?.IsConnectedProvider(providerId) ?? false;

        public static string[] GetConnectedProviders() =>
            _authController?.GetConnectedProviders() ?? Array.Empty<string>();

        public static async UniTask<UserInfo> LoginAsync()
        {
            if (_authController == null) return new UserInfo();
            return await _authController.LoginAsync();
        }

        public static async UniTask<UserInfo> SignInAnonymousAsync()
        {
            var auth = FirebaseAuth.DefaultInstance;
            var result = await auth.SignInAnonymouslyAsync();
            Debug.Log($"anonymous: {result.Credential.Provider}");
            foreach (var userInfo in result.User.ProviderData)
            {
                Debug.Log(userInfo.ProviderId);
            }

            Debug.Log($"is anonymous: {result.User.IsAnonymous}");

            return new UserInfo(result.User);
        }

        /// <summary>
        /// 단일 계정 연결.
        /// 기존에 SignIn되어 있는 공급자가 있다면 SignOut처리 됨
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns></returns>
        public static async UniTask<UserInfo> SignInAsync(string providerId)
        {
            if (_authController == null) return new UserInfo();
            return await _authController.SignInAsync(providerId);
        }

        public static async UniTask<string> LinkAsync(string providerId)
        {
            if (_authController == null) return null;
            return await _authController.LinkAsync(providerId);
        }

        public static async UniTask<bool> SignOutAsync(string providerId)
        {
            if (_authController == null) return false;
            return await _authController.SignOutAsync(providerId);
        }
    }
}
#endif