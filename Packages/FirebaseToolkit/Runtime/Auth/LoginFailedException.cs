using System;

namespace FirebaseToolkit.Auth
{
    public enum LoginFailReason
    {
        InvalidCredential,
        NotSupportProvider
    }

    public class LoginFailedException : Exception
    {
        public readonly LoginFailReason Reason;

        public LoginFailedException(LoginFailReason reason)
            : base($"login process failed: {reason.ToString()}")
        {
            Reason = reason;
        }
    }
}