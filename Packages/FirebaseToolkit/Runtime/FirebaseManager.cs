#define FBTK_LOG

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Firebase;
using Debug = UnityEngine.Debug;

namespace FirebaseToolkit
{
    public partial class FirebaseConfig
    {
    }

    public static partial class FirebaseManager
    {
        private static FirebaseApp _app;
        private static bool IsInitialized => _app != null;

        public static async Task InitializeAsync(FirebaseConfig config)
        {
            var result = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (result == DependencyStatus.Available)
            {
                _app = FirebaseApp.DefaultInstance;
                AppInitialized(config);
            }
            else
            {
                throw new AppInitializeFailedException(result);
            }
        }

        static partial void AppInitialized(FirebaseConfig config);

        [Conditional("FBTK_LOG")]
        internal static void Log(string message)
        {
            Debug.Log($"[FBTK] {message}");
        }

        [Conditional("FBTK_LOG")]
        internal static void Log(string category, string message)
        {
            Debug.Log($"[FBTK] [{category}] {message}");
        }
    }


    public class AppInitializeFailedException : Exception
    {
        public AppInitializeFailedException(DependencyStatus status)
            : base($"Could not resolve all Firebase dependencies: {status}")
        {
        }
    }
}