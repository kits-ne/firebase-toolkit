using Firebase.Auth;

namespace FirebaseToolkit.Auth
{
    public readonly struct UserInfo
    {
        public readonly string Id;
        public readonly string Email;

        public UserInfo(string id, string email)
        {
            Id = id;
            Email = email;
        }

        public UserInfo(FirebaseUser user)
        {
            Id = user.UserId;
            Email = user.Email;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Id);
        }
    }
}