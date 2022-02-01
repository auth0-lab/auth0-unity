# auth0-unity-sdk
An experimental Auth0 SDK for Unity platform.

## Requirements

* Take a look to the [Auth0 Device Flow's prerequisites](https://auth0.com/docs/quickstart/native/device/01-login#prerequisites) section.
* Unity 2020.x (or later)

## Setup

* **Option 1 _(not available yet)_:** Install and import the Auth0 package from [Unity Assets Store](https://assetstore.unity.com/).
* **Option 2:** Download [Auth0UnitySDK.unitypackage](https://github.com/auth0-lab/auth0-unity-sdk/raw/main/Auth0UnitySDK-v0.0.1.unitypackage) and import it in your proyect as a `Custom Package`.

<img width="500" src="https://user-images.githubusercontent.com/178506/151574518-1a5bad47-cb07-433d-998a-5e1398b8f181.png">

## Device Flow

1. Go to `Assets/Auth0/Scripts/Runtime/AuthManager.cs` and set the following settings:

```cs
// TODO: use your favorite strategy to load the Auth0 configuration (ie, RemoteSettings)
this.Settings = new Settings
{
    Domain = "", // "acme.auth0.com"
    ClientId = "",
    Scope = "openid profile offline_access",
    Audience = ""
};
```

* `Domain`, `ClientId` and `Scope` are mandatory.
* When authentication is performed with the `offline_access` scope included, it returns a refresh token that can be used by `AuthManager` to request a new user token, without forcing the user to perform authentication again.

2. Include the `Assets/Auth0/Prefabs/DeviceFlow` prefab in the section/canvas that you consider.

<img width="400" src="https://user-images.githubusercontent.com/178506/151596725-e39b3c70-689f-4d07-803d-906ebfb96f44.png">

Alternativelly, if you don't want to use this prefab to show verification uri, user code and result, just add the `Assets/Auth0/Scripts/DeviceFlow` script in your canvas/panel and specify your own UI components:

<img width="500" src="https://user-images.githubusercontent.com/178506/151962282-477f71a5-8aff-47fa-9318-a6347aa25130.png">

## Auth Manager

The `AuthManager` is a singleton instance that exposes the following properties:

* `AuthManager.Instance.Auth0`: Auth0 Authentication API client.
* `AuthManager.Instance.Credentials`: A utility class to streamline the process of storing and renewing credentials. You can access the `AccessToken` or `IdToken` properties from the `Credentials` instance.
    - `bool HasValidCredentials()`: Stored credentials are considered valid if they have not expired or can be refreshed. Useful to check if a user has already logged in.
    - `void ClearCredentials()`: Remove the stored credentials. Useful to log the user out of your app.
    - `Task<Credentials> GetCredentials()`: If the access token has expired, the manager automatically uses the refresh token and renews the credentials. New credentials are be stored for future access.
    - `void SaveCredentials(Credentials)`: Save the credentials (for example, the ones obtained during device flow).

### Common scenarios

1. Change UI based on current credentials:

```c#
public void UpdateLoginStatus()
{
    var loggedIn = AuthManager.Instance.credentials.HasValidCredentials();

    if (loggedIn)
    {
        // Show a welcome message and the SignOut button
        var creds = await AuthManager.Instance.credentials.GetCredentials();
        var userInfo = await AuthManager.Instance.Auth0.GetUserInfo(creds.AccessToken);

        welcomeText.text = String.Format("Welcome back {0}!", userInfo.FullName);
        signOutButton.SetActive(true);
    }
    else
    {
        // Show the SignIn button, which points to the section that contains the `DeviceFlow` prefab.
        signInButton.SetActive(true);
    }
}
```

2. Logout

```cs
public void SignOutBtn()
{
    AuthManager.Instance.credentials.ClearCredentials();
    this.UpdateLoginStatus();
}
```
