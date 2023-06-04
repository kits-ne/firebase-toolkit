using System;
using Cysharp.Threading.Tasks;
using Firebase;

namespace FirebaseToolkit
{
    public partial class FirebaseConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public bool SkipAppInit = false;
    }

    public static partial class FirebaseManager
    {
        private static FirebaseApp _app;
        private static bool IsInitialized => _app != null;

        public static async UniTask InitializeAsync(FirebaseConfig config)
        {
            var result = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (result == DependencyStatus.Available)
            {
                if (!config.SkipAppInit)
                {
                    _app = FirebaseApp.DefaultInstance;
                }

                AppInitialized(config);
            }
            else
            {
                throw new AppInitializeFailedException(result);
            }
        }

        static partial void AppInitialized(FirebaseConfig config);
    }


    public class AppInitializeFailedException : Exception
    {
        public AppInitializeFailedException(DependencyStatus status)
            : base($"Could not resolve all Firebase dependencies: {status}")
        {
        }
    }
}