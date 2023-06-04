#if FBTK_AUTH
using System;
using Cysharp.Threading.Tasks;
using FirebaseToolkit.Auth;

namespace FirebaseToolkit
{
    public partial class FirebaseConfig
    {
        public readonly AuthConfig Auth = new AuthConfig();
    }

    public static partial class FirebaseManager
    {
        private static IAuthController _authController;

        static partial void AppInitialized(FirebaseConfig config)
        {
#if UNITY_EDITOR
            _authController = new EditorAuthController(config.Auth);
#else
            _authController = new AuthController(FirebaseAuth.DefaultInstance, config.Auth);
#endif
        }

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

        public static async UniTask<UserInfo> SignInAsync(string providerId)
        {
            if (_authController == null) return new UserInfo();
            return await _authController.SignInAsync(providerId);
        }


        public static async UniTask<bool> SignOutAsync(string providerId)
        {
            if (_authController == null) return false;
            return await _authController.SignOutAsync(providerId);
        }
    }
}
#endif