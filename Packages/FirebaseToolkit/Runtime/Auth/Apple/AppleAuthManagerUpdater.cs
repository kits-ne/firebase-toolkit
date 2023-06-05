using System;
using AppleAuth;
using UnityEngine;

namespace FirebaseToolkit.Auth.Apple
{
    public class AppleAuthManagerUpdater : MonoBehaviour
    {
        public IAppleAuthManager Manager = null;

        private void Awake()
        {
            Debug.Log($"created {nameof(AppleAuthManagerUpdater)}");
        }

        private void Update()
        {
            Manager?.Update();
        }
    }
}