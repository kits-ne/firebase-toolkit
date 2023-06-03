#if FBTK_AUTH
using System.Threading.Tasks;
using Firebase.Auth;
using FirebaseToolkit.Auth;

namespace FirebaseToolkit
{
    public partial class FirebaseConfig
    {
        public readonly AuthConfig Auth = new AuthConfig();
    }

    public static partial class FirebaseManager
    {
        private static FirebaseAuth _auth;
        private static AuthController _authController;

        static partial void AppInitialized(FirebaseConfig config)
        {
            _auth = FirebaseAuth.DefaultInstance;
            _authController = new AuthController(config.Auth);
        }

        public static bool IsSupportCredential(string providerId) =>
            _authController?.IsSupportCredential(providerId) ?? false;

        public static async Task<UserInfo> Login()
        {
            if (_authController == null) return new UserInfo();
            return await _authController.Login(_auth);
        }

        public static async Task<UserInfo> SignIn(string providerId)
        {
            if (_authController == null) return new UserInfo();
            return await _authController.SignIn(_auth, providerId);
        }
    }
}
#endif