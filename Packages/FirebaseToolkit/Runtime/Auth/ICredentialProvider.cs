using System.Threading.Tasks;
using Firebase.Auth;

namespace FirebaseToolkit.Auth
{
    public interface ICredentialProvider
    {
        bool IsSupported { get; }
        string ProviderId { get; }
        Task<bool> Validate(IUserInfo userInfo);
        Task<Credential> SignIn();
    }
}