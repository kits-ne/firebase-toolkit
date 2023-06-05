using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using FirebaseToolkit;
using FirebaseToolkit.Auth;
using FirebaseToolkit.Auth.Apple;
using FirebaseToolkit.Auth.Google;
using UnityEngine;
using UnityEngine.UI;


public class LoginController : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button deleteButton;

    [SerializeField] private GameObject signInButtons;
    [SerializeField] private Button signInAnonymousButton;
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

        FirebaseAuth.DefaultInstance.StateChanged += (sender, args) =>
        {
            var auth = FirebaseAuth.DefaultInstance;
            if (auth is {CurrentUser: { }})
            {
                Debug.Log($"state changed: {auth.CurrentUser.UserId} {auth.CurrentUser.IsAnonymous}");
            }
            else
            {
                Debug.Log($"state changed: user is null");
            }
        };
    }

    private void OnInitialized()
    {
        loginButton.gameObject.SetActive(true);
        loginButton.onClick.AddListener(OnClickLogin);
        logoutButton.onClick.AddListener(OnClickLogout);
        deleteButton.onClick.AddListener(OnClickDelete);

        signInAnonymousButton.onClick.AddListener(OnClickSignInAnonymous);

        signInGoogleButton.onClick.AddListener(OnClickSignInGoogle);
        signOutGoogleButton.onClick.AddListener(OnClickSignOutGoogle);
        linkGoogleButton.onClick.AddListener(OnClickLinkGoogle);

        signInAppleButton.onClick.AddListener(OnClickSignInApple);
        signOutAppleButton.onClick.AddListener(OnClickSignOutApple);
        linkAppleButton.onClick.AddListener(OnClickLinkApple);
    }

    private void OnClickSignInAnonymous()
    {
        try
        {
            FirebaseManager.SignInAnonymousAsync().ContinueWith(_ =>
            {
                Debug.Log($"anonymous success: {_.IsAnonymous} {_.Id}");
                UpdateSignInOutButtons();
            });
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void OnClickDelete()
    {
        var auth = FirebaseAuth.DefaultInstance;
        var user = auth.CurrentUser;

        if (user != null)
        {
            try
            {
                user.DeleteAsync().ContinueWith(_ =>
                {
                    auth.SignOut();
                    Debug.Log("delete success");
                    if (FirebaseAuth.DefaultInstance.CurrentUser == null)
                    {
                        Debug.Log("user is null");
                    }
                    else
                    {
                        Debug.Log($"user is not null");
                    }

                    UpdateSignInOutButtons();
                });
            }
            catch (Exception)
            {
                Debug.Log("delete failed");
            }
        }
    }

    private void OnClickLogout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        UpdateSignInOutButtons();
    }


    private void OnClickLogin()
    {
        LoginAsync().Forget();
    }

    private void OnClickSignInGoogle()
    {
        FirebaseManager.SignInAsync(GoogleCredentialProvider.Id).ContinueWith(_ =>
        {
            Debug.Log($"sign in with google success: {_.Id}");
            UpdateSignInOutButtons();
        });
    }

    private void OnClickSignInApple()
    {
        FirebaseManager.SignInAsync(AppleCredentialProvider.Id).ContinueWith(_ =>
        {
            Debug.Log($"sign in with apple success: {_.Id}");
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
            Debug.Log($"login success: {user.IsAnonymous}");
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

        var user = FirebaseManager.GetUser();
        signInAnonymousButton.gameObject.SetActive(!user.IsValid());


        signInGoogleButton.gameObject.SetActive(
            !user.IsValid()
            && !connectedProviders.Contains(GoogleCredentialProvider.Id)
        );
        signOutGoogleButton.gameObject.SetActive(
            user.IsValid()
            && connectedProviders.Contains(GoogleCredentialProvider.Id)
        );


        signInAppleButton.gameObject.SetActive(
            !user.IsValid()
            && !connectedProviders.Contains(AppleCredentialProvider.Id)
        );
        signOutAppleButton.gameObject.SetActive(
            user.IsValid()
            && connectedProviders.Contains(AppleCredentialProvider.Id)
        );

        linkGoogleButton.gameObject.SetActive(
            user.IsValid()
            && !connectedProviders.Contains(GoogleCredentialProvider.Id)
        );
        linkAppleButton.gameObject.SetActive(
            user.IsValid()
            && !connectedProviders.Contains(AppleCredentialProvider.Id)
        );
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
            Debug.LogError($"link failed: {e}");
        }
    }
}