using Cysharp.Threading.Tasks;
using FirebaseToolkit;
using FirebaseToolkit.Auth;
using FirebaseToolkit.Auth.Apple;
using FirebaseToolkit.Auth.Google;
using UnityEngine;
using UnityEngine.UI;

public class SignInSample : MonoBehaviour
{
    public string webClientId;

    public Button loginButton;
    public Button googleButton;
    public Button appleButton;

    private void Start()
    {
        googleButton.onClick.AddListener(() => { SignIn(GoogleCredentialProvider.Id).Forget(); });
        googleButton.gameObject.SetActive(false);

        appleButton.onClick.AddListener(() => { SignIn(AppleCredentialProvider.Id).Forget(); });
        appleButton.gameObject.SetActive(false);
        loginButton.onClick.AddListener(() => { Login().Forget(); });
    }

    public async UniTaskVoid Login()
    {
        var config = new FirebaseConfig();
// Editor
#if UNITY_EDITOR
        config.Auth.EditorUser = new UserInfo("test4058", "unity-editor@email.com");
#endif
        // Google SignIn
        config.Auth.AddCredentialProvider(new GoogleCredentialProvider(webClientId));
        // Sign in with apple
        config.Auth.AddCredentialProvider(new AppleCredentialProvider());

        await FirebaseManager.InitializeAsync(config);

        UserInfo user;
        try
        {
            user = await FirebaseManager.Login().AsUniTask();
        }
        catch (LoginFailedException e)
        {
            Debug.Log(e.Reason.ToString());
            switch (e.Reason)
            {
                case LoginFailReason.InvalidCredential:
                    EnableSignInProviders();
                    break;
            }

            return;
        }

        if (user.IsValid())
        {
            Debug.Log($"sample login success: {user.Id}");
            gameObject.SetActive(false);
            return;
        }

        Debug.Log("login failed -> sign in with...");

        loginButton.gameObject.SetActive(false);
        EnableSignInProviders();
    }

    private void EnableSignInProviders()
    {
        appleButton.gameObject.SetActive(FirebaseManager.IsSupportCredential(AppleCredentialProvider.Id));
        googleButton.gameObject.SetActive(FirebaseManager.IsSupportCredential(GoogleCredentialProvider.Id));
    }

    private async UniTaskVoid SignIn(string providerId)
    {
        googleButton.gameObject.SetActive(false);
        appleButton.gameObject.SetActive(false);

        UserInfo user = new UserInfo();
        try
        {
            user = await FirebaseManager.SignIn(providerId);
        }
        catch (SignInFailedException e)
        {
            Debug.Log($"sign in failed exception: {e}");
        }

        if (user.IsValid())
        {
            Debug.Log($"sign in with {providerId}: {user.Id}");
        }
        else
        {
            Debug.Log($"sign in failed: {providerId}");
            EnableSignInProviders();
        }
    }
}