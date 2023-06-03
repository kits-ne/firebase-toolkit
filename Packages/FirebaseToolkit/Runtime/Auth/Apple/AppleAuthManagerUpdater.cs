using AppleAuth;
using UnityEngine;

namespace FirebaseToolkit.Auth.Apple
{
    public class AppleAuthManagerUpdater : MonoBehaviour
    {
        public IAppleAuthManager Manager = null;

        private void Update()
        {
            Manager?.Update();
        }
    }
}