using System;

namespace FirebaseToolkit.Auth
{
    public class LinkFailedException : Exception
    {
        public readonly string ProviderId;

        public LinkFailedException(string providerId, Exception e) : base($"link failed: {providerId}", e)
        {
            ProviderId = providerId;
        }
    }
}