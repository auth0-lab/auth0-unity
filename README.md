# Auth0 Unity SDK

[![FOSSA Status](https://app.fossa.com/api/projects/custom%2B4989%2Fgit%40github.com%3Aauth0-lab%2Fauth0-unity-sdk.git.svg?type=shield)](https://app.fossa.com/projects/custom%2B4989%2Fgit%40github.com%3Aauth0-lab%2Fauth0-unity-sdk.git?ref=badge_shield)

Unity platform toolkit for consuming the Auth0 Authentication API.

> :warning: This is an experimental library and has not had a complete security review. As we we learn and iterate, please be aware that releases may contain breaking changes.

## Requirements

* Unity 2020.x (or later).
* In order to use one of the pre-defined assets, you need to import [Oculus Integration v37](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022) from [Unity Asset Store](https://assetstore.unity.com/).

## Setup

* **Option 1 _(not available yet)_:** Install and import the Auth0 package from [Unity Assets Store](https://assetstore.unity.com/).
* **Option 2:** Download [Auth0UnitySDK.unitypackage](https://github.com/auth0-lab/auth0-unity-sdk/raw/main/Auth0UnitySDK-v0.2.0.unitypackage) and import it into your proyect as a `Custom Package`.

<p align="center"><img width="500" src="https://user-images.githubusercontent.com/178506/151574518-1a5bad47-cb07-433d-998a-5e1398b8f181.png"></p>

> If after import, you got the _"Multiple precompiled assemblies with the same name Newtonsoft.Json.dll included or the current platform. Only one assembly with the same name is allowed per platform."_ error, it could be caused by an old version of the `Version Control` package, which is installed by default by some Unity project templates. Please, go to `Window -> Package Manager -> Packages: In Project` and update it to `v1.15.12` (or later), or just remove it if you have no plans to use it.

Then, go to `Assets/Auth0/Runtime/AuthManager.cs` and to set your Auth0 settings:

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
* `Audience` is required in case you need an access token to call to your API.

## Device Flow

Before start, check the [Auth0 Device Flow's prerequisites](https://auth0.com/docs/quickstart/native/device/01-login#prerequisites).

### Prefabs

#### DeviceFlowRaw

Include the `Assets/Auth0/Prefabs/DeviceFlowRaw` prefab into the section/canvas that you consider.

<p align="center"><img width="400" src="https://user-images.githubusercontent.com/178506/151596725-e39b3c70-689f-4d07-803d-906ebfb96f44.png"></p>

#### DeviceFlowPrompt

Include the `Assets/Auth0/Prefabs/DeviceFlowPrompt` prefab in your scene and activate it when you need it.

<p align="center"><img width="400" src="https://user-images.githubusercontent.com/178506/154989639-baabe605-49e6-4e53-82a0-e0999466e4e7.png"></p>

### Scenes

The package includes some sample scenes (`Assets/Auth0/Scenes`) to show how to use the described prefabs.

### Scripts

If you don't want to use the included prefabs to show instructions (verification uri, user code) and result, just add the `Assets/Auth0/Scripts/DeviceFlow` script in your canvas/panel and specify your own UI components:

<p align="center"><img width="500" src="https://user-images.githubusercontent.com/178506/151962282-477f71a5-8aff-47fa-9318-a6347aa25130.png"></p>

* `Instructions`: This canvas contains the instructions to complete the flow, including `Verification Uri` and `User Code` components. This is deactivated by the script when a result (successful or failed) has to be shown to end-user. 
* `Verification Uri`: A text component to set the verification uri returned by Auth0 (usually it looks like `https://{your_auth0_domain}/activate`).
* `User Code`: A text component to set the user code returned by Auth0 (`****-****`).
* `Result`: A text component to show a confirmation message after end-user finished with the flow or an error if something unexpected happens.

## Auth Manager

The `AuthManager` is a singleton instance that exposes the following properties:

* `AuthManager.Instance.Auth0`:
    - Exposes an instance of the [.NET client library for Auth0 Authentication API](https://auth0.github.io/auth0.net/api/Auth0.AuthenticationApi.AuthenticationApiClient.html#methods).
    - Also includes a Device Flow wrapper to poll the token endpoint to request a token (`Task<AccessTokenResponse> ExchangeDeviceCodeAsync(string clientId, string deviceCode, int retryInterval`).
* `AuthManager.Instance.Credentials`: A utility class to streamline the process of storing and renewing credentials. You can access the `AccessToken` or `IdToken` properties from the `Credentials` instance.
    - `bool HasValidCredentials()`: Stored credentials are considered valid if they have not expired or can be refreshed. Useful to check if a user has already logged in.
    - `void ClearCredentials()`: Remove the stored credentials. Useful to log the user out of your app.
    - `void SaveCredentials(AccessTokenResponse, scope)`: Save the credentials obtained during authentication in the manager.
    - `Task<Credentials> GetCredentials()`: If the access token has expired, the manager automatically uses the refresh token and renews the credentials. New credentials are be stored for future access. Credentials contains the following properties:
      - `AccessToken (string)`: Access token for specified API (audience).
      - `ExpiresAt (DateTime)`: Expiration date of the Access Token.
      - `Scope (string)`: Access Token's granted scope.
      - `RefreshToken (string)`: Refresh Token that can be used to request new tokens without signing in again.
      - `IdToken (string)`: Identifier Token with user information.
      - `User (UserInfo)`: Decoded IdToken.

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
        var userInfo = await AuthManager.Instance.Auth0.GetUserInfoAsync(creds.AccessToken);

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

## What is Auth0?

Auth0 helps you to:

* Add authentication with [multiple authentication sources](https://auth0.com/docs/identityproviders), either social like **Google, Facebook, Microsoft Account, LinkedIn, GitHub, Twitter, Box, Salesforce, among others**, or enterprise identity systems like **Windows Azure AD, Google Apps, Active Directory, ADFS or any SAML Identity Provider**.
* Add authentication through more traditional [username/password databases](https://auth0.com/docs/connections/database/custom-db).
* Add support for [linking different user accounts](https://auth0.com/docs/link-accounts) with the same user.
* Support for generating signed [JSON Web Tokens](https://auth0.com/docs/jwt) to call your APIs and **flow the user identity** securely.
* Analytics of how, when, and where users are logging in.
* Pull data from other sources and add it to the user profile, through [JavaScript rules](https://auth0.com/docs/rules/current).

## Issue Reporting

If you have found a bug or if you have a feature request, please report them at this repository issues section. Please do not report security vulnerabilities on the public GitHub issue tracker. The [Responsible Disclosure Program](https://auth0.com/whitehat) details the procedure for disclosing security issues. This is an Auth0 Lab Experiment, it may not get updated.

## Author

[Auth0Lab](https://github.com/auth0-lab) - The experimentation arm of [Auth0](https://auth0.com/).

## License

This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for more info. Auth0Lab experiments should be reviewed with a closer eye than Auth0 repos.
