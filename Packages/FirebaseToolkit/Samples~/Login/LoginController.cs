using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using FirebaseToolkit;
using FirebaseToolkit.Auth;
using FirebaseToolkit.Auth.Apple;
using FirebaseToolkit.Auth.Google;
using UnityEngine;
using UnityEngine.UI;


public class LoginController : MonoBehaviour
{
    [SerializeField] private Button loginButton;

    [SerializeField] private GameObject signInButtons;
    [SerializeField] private Button signInGoogleButton;
    [SerializeField] private Button signInAppleButton;

    [SerializeField] private GameObject signOutButtons;
    [SerializeField] private Button signOutGoogleButton;
    [SerializeField] private Button signOutAppleButton;

    [SerializeField] private GameObject linkButtons;
    [SerializeField] private Button linkGoogleButton;
    [SerializeField] private Button linkAppleButton;

    public string webClientId = "webClientId";
    public bool skipAppInit = true;

    void Start()
    {
        loginButton.gameObject.SetActive(false);
        EnableButtons(false);
        InitializeAsync().ContinueWith(OnInitialized);
    }

    private void EnableButtons(bool isEnable)
    {
        signInButtons.gameObject.SetActive(isEnable);
        signOutButtons.gameObject.SetActive(isEnable);
        linkButtons.gameObject.SetActive(isEnable);
    }

    private async UniTask InitializeAsync()
    {
        var config = new FirebaseConfig()
        {
            SkipAppInit = skipAppInit
        };
        config.Auth.AddCredentialProvider(new GoogleCredentialProvider(webClientId));
        config.Auth.AddCredentialProvider(new AppleCredentialProvider());

        try
        {
            await FirebaseManager.InitializeAsync(config);
        }
        catch (Exception)
        {
            Debug.Log("init failed");
        }
    }

    private void OnInitialized()
    {
        loginButton.gameObject.SetActive(true);
        loginButton.onClick.AddListener(OnClickLogin);

        signInGoogleButton.onClick.AddListener(OnClickSignInGoogle);
        signOutGoogleButton.onClick.AddListener(OnClickSignOutGoogle);
        linkGoogleButton.onClick.AddListener(OnClickLinkGoogle);

        signInAppleButton.onClick.AddListener(OnClickSignInApple);
        signOutAppleButton.onClick.AddListener(OnClickSignOutApple);
        linkAppleButton.onClick.AddListener(OnClickLinkApple);
    }


    private void OnClickLogin()
    {
        LoginAsync().Forget();
    }

    private void OnClickSignInGoogle()
    {
        FirebaseManager.SignInAsync(GoogleCredentialProvider.Id).ContinueWith(_ =>
        {
            Debug.Log("sign in with google success");
            UpdateSignInOutButtons();
        });
    }

    private void OnClickSignInApple()
    {
        FirebaseManager.SignInAsync(AppleCredentialProvider.Id).ContinueWith(_ =>
        {
            Debug.Log("sign in with apple success");
            UpdateSignInOutButtons();
        });
    }


    private async UniTask LoginAsync()
    {
        UserInfo user = new UserInfo();

        try
        {
            user = await FirebaseManager.LoginAsync();
        }
        catch (LoginFailedException e)
        {
            Debug.LogException(e);
        }

        if (!user.IsValid())
        {
            Debug.Log("invalid user. go to sign in step");
        }
        else
        {
            Debug.Log("login success");
        }

        loginButton.gameObject.SetActive(false);
        EnableButtons(true);
        UpdateSignInOutButtons();
    }


    private void UpdateSignInOutButtons()
    {
        var connectedProviders = FirebaseManager.GetConnectedProviders();
        foreach (var provider in connectedProviders)
        {
            Debug.Log($"connected: {provider}");
        }

        var googleSignIn = FirebaseManager.IsSupportCredential(GoogleCredentialProvider.Id)
                           && !connectedProviders.Contains(GoogleCredentialProvider.Id);
        signInGoogleButton.gameObject.SetActive(googleSignIn);
        signOutGoogleButton.gameObject.SetActive(!googleSignIn);


        var appleSignIn = FirebaseManager.IsSupportCredential(AppleCredentialProvider.Id)
                          && !connectedProviders.Contains(AppleCredentialProvider.Id);
        signInAppleButton.gameObject.SetActive(appleSignIn);
        signOutAppleButton.gameObject.SetActive(!appleSignIn);

        linkGoogleButton.gameObject.SetActive(googleSignIn);
        linkAppleButton.gameObject.SetActive(appleSignIn);
    }

    private void OnClickSignOutGoogle()
    {
        FirebaseManager.SignOutAsync(GoogleCredentialProvider.Id).ContinueWith(_ =>
        {
            Debug.Log(_ ? "sign out google success" : "sign out google failure");
            UpdateSignInOutButtons();
        });
    }

    private void OnClickSignOutApple()
    {
        FirebaseManager.SignOutAsync(AppleCredentialProvider.Id).ContinueWith(_ =>
        {
            Debug.Log(_ ? "sign out apple success" : "sign out apple failure");
            UpdateSignInOutButtons();
        });
    }


    private void OnClickLinkGoogle()
    {
        try
        {
            FirebaseManager.LinkAsync(GoogleCredentialProvider.Id).ContinueWith(_ =>
            {
                Debug.Log($"link success: {_}");
                UpdateSignInOutButtons();
            });
        }
        catch (LinkFailedException e)
        {
            Debug.LogException(e);
            Debug.LogException(e.InnerException);
        }
    }

    private void OnClickLinkApple()
    {
        try
        {
            FirebaseManager.LinkAsync(AppleCredentialProvider.Id)
                .ContinueWith(_ =>
                {
                    Debug.Log($"link success: {_}");
                    UpdateSignInOutButtons();
                });
        }
        catch (LinkFailedException e)
        {
            Debug.LogException(e);
            Debug.LogException(e.InnerException);
        }
    }
}