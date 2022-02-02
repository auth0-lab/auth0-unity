# auth0-unity-sdk
An experimental Auth0 SDK for Unity platform.

## Requirements

* Take a look to the [Auth0 Device Flow's prerequisites](https://auth0.com/docs/quickstart/native/device/01-login#prerequisites) section.
* Unity 2020.x (or later).

## Setup

* **Option 1 _(not available yet)_:** Install and import the Auth0 package from [Unity Assets Store](https://assetstore.unity.com/).
* **Option 2:** Download [Auth0UnitySDK.unitypackage](https://github.com/auth0-lab/auth0-unity-sdk/raw/main/Auth0UnitySDK-v0.0.3.unitypackage) and import it in your proyect as a `Custom Package`.

<img width="500" src="https://user-images.githubusercontent.com/178506/151574518-1a5bad47-cb07-433d-998a-5e1398b8f181.png">

> If after import, you got the _"Multiple precompiled assemblies with the same name Newtonsoft.Json.dll included or the current platform. Only one assembly with the same name is allowed per platform."_ error, please, go to `Window -> Package Manager -> Packages: In Project` and update `Version Control` to `v1.15.12` (or later), or remove it, if you aren't using it.

## Device Flow

Before you start you need to configure your Device flow on your Auth0 tenant. If you didn't do this yet check the [Auth0 Device Flow's prerequisites](https://auth0.com/docs/quickstart/native/device/01-login#prerequisites)

1. Now, go to `Assets/Auth0/Runtime/AuthManager.cs` and set the following settings:

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

Alternativelly, if you don't want to use this prefab to show instructions (verification uri, user code) and result, just add the `Assets/Auth0/Scripts/DeviceFlow` script in your canvas/panel and specify your own UI components:

<img width="500" src="https://user-images.githubusercontent.com/178506/151962282-477f71a5-8aff-47fa-9318-a6347aa25130.png">

* `Instructions`: This canvas contains the instructions to complete the flow, including `Verification Uri` and `User Code` components. This is deactivated by the script when a result (successful or failed) has to be shown to end-user. 
* `Verification Uri`: A text component to set the verification uri returned by Auth0 (usually it looks like `https://{your_auth0_domain}/activate`).
* `User Code`: A text component to set the user code returned by Auth0 (`****-****`).
* `Result`: A text component to show a confirmation message after end-user finished with the flow or an error if something unexpected happens.

## Auth Manager

The `AuthManager` is a singleton instance that exposes the following properties:

* `AuthManager.Instance.Auth0`: Auth0 Authentication API client.
* `AuthManager.Instance.Credentials`: A utility class to streamline the process of storing and renewing credentials. You can access the `AccessToken` or `IdToken` properties from the `Credentials` instance.
    - `bool HasValidCredentials()`: Stored credentials are considered valid if they have not expired or can be refreshed. Useful to check if a user has already logged in.
    - `void ClearCredentials()`: Remove the stored credentials. Useful to log the user out of your app.
    - `Task<Credentials> GetCredentials()`: If the access token has expired, the manager automatically uses the refresh token and renews the credentials. New credentials are be stored for future access.
    - `void SaveCredentials(AccessTokenResponse, scope)`: Save the credentials obtained during authentication in the manager.

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

## Beta Terms of Service

This SDK is an Auth0 Lab experiment and is subject to our [Beta Terms of Service](https://cdn.auth0.com/website/legal/terms/beta-service-terms-11-18-19.pdf?_ga=2.178667751.270615629.1602600152-1580183395.1598368988). The code is released "as-is" and may or may not be updated in the future. By using it, you agree to the Beta Terms. Bug fixes, and issue callouts are welcome, but may not be merged in a timely manner.
