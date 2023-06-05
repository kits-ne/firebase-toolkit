using System;
using Cysharp.Threading.Tasks;
using Firebase.Auth;

namespace FirebaseToolkit.Auth
{
    public class AnonymousCredentialProvider : ICredentialProvider
    {
        public bool IsSupported => FirebaseAuth.DefaultInstance.CurrentUser == null;
        public string ProviderId => "anonymous";

        public UniTask<bool> Validate(IUserInfo userInfo)
        {
            return UniTask.FromResult(true);
        }

        public async UniTask<Credential> SignIn()
        {
            var auth = FirebaseAuth.DefaultInstance;
            var result = await auth.SignInAnonymouslyAsync();
            return result.Credential;
        }
    }
}