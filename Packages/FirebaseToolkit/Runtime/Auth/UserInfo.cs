using Firebase.Auth;

namespace FirebaseToolkit.Auth
{
    public readonly struct UserInfo
    {
        public readonly string Id;
        public readonly string Email;
        public readonly bool IsAnonymous;

        public UserInfo(string id, string email, bool isAnonymous = true)
        {
            Id = id;
            Email = email;
            IsAnonymous = isAnonymous;
        }

        public UserInfo(FirebaseUser user)
        {
            Id = user?.UserId;
            Email = user?.Email;
            IsAnonymous = user?.IsAnonymous ?? false;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Id);
        }
    }
}