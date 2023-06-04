using Cysharp.Threading.Tasks;
using Firebase.Auth;

namespace FirebaseToolkit.Auth
{
    public interface ICredentialProvider
    {
        bool IsSupported { get; }
        string ProviderId { get; }
        UniTask<bool> Validate(IUserInfo userInfo);
        UniTask<Credential> SignIn();
    }
}