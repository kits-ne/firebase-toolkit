using Cysharp.Threading.Tasks;

namespace FirebaseToolkit.Auth
{
    public interface IAuthController
    {
        UniTask<UserInfo> LoginAsync();
        UniTask<UserInfo> SignInAsync(string providerId);
        UniTask<string> LinkAsync(string providerId);
        UniTask<bool> SignOutAsync(string providerId);
        bool IsSupportCredential(string providerId);
        bool IsConnectedProvider(string providerId);
        string[] GetConnectedProviders();
    }
}