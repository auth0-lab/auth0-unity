![banner](https://cdn.auth0.com/website/octo/sdkvr/readme-banner.png)

# Auth0 Lab Unity SDK

[![FOSSA Status](https://app.fossa.com/api/projects/custom%2B4989%2Fgit%40github.com%3Aauth0-lab%2Fauth0-unity-sdk.git.svg?type=shield)](https://app.fossa.com/projects/custom%2B4989%2Fgit%40github.com%3Aauth0-lab%2Fauth0-unity-sdk.git?ref=badge_shield)

The Auth0 Lab Unity SDK is an experimental platform toolkit that makes it easy to add sign in and other identity features to Unity applications, leveraging Auth0 Authentication API.
Although the SDK can be used with any Unity application, this release emphasizes an exploration of the authentication experience in Virtual Reality (VR) applications.

> :warning: This is a PROOF OF CONCEPT experimental library and has not had a complete security review. As we learn and iterate, please be aware that releases may contain breaking changes.

</br>

## Requirements

* [Unity](https://unity.com/download) Editor 2020.x (or later).
* The VR prefab included in Auth0 Unity SDK is optimized for Meta Oculus VR devices and requires you to to import [Oculus Integration v37](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022) from [Unity Asset Store](https://assetstore.unity.com/).

</br>

## Setup
The Auth0 Unity SDK is packaged and distributed as a Unity Asset Package. You have two options for including it in your project.
* **Option 1 _(not available yet)_:** Install and import the Auth0 package from [Unity Assets Store](https://assetstore.unity.com/).
* **Option 2:** Download [Auth0UnitySDK.unitypackage](https://github.com/auth0-lab/auth0-unity/raw/main/Auth0UnitySDK-v0.3.0.unitypackage) and import it into your project as a `Custom Package`.

<p align="center"><img width="500" src="https://user-images.githubusercontent.com/178506/151574518-1a5bad47-cb07-433d-998a-5e1398b8f181.png"></p>

> Warning. Some Unity project templates install an old version of the `Version Control` package. If that applies to your project, upon importing the Auth0 Unity SDK you will get the _"Multiple precompiled assemblies with the same name Newtonsoft.Json.dll included or the current platform. Only one assembly with the same name is allowed per platform."_ error.
This is easily fixed by updating the `Version Control` package (go to `Window -> Package Manager -> Packages: In Project` and update it to `v1.15.12` or later), or by simply removing it if your project doesn't actually need that package.

</br>

## Getting Started
The Auth0 Unity SDK offers authentication functionality to Unity apps by wrapping the Auth0 .NET authentication SDK in prefabs that can be easily integrated in Unity scenes.
Using the SDK requires completing two tasks: initializing the SDK with parameters connecting the app to an application registration in an Auth0 tenant, and instantiating one of the prefabs that will expose the authentication experience to the app user.


### Initializing the Auth0 Unity SDK
In order to use Auth0 to handle authentication, the application needs to be registered in an Auth0 tenant. If you don't have one, you can sign up [here](https://auth0.com/docs/get-started/auth0-overview/create-tenants).
Registering an application in an Auth0 tenant is easy: you can simply follow the instructions [here](https://auth0.com/docs/quickstart/native/device/01-login#prerequisites).
> The Auth0 Unity SDK take care of all details, but if you are curious: the experience offered here implements the authentication flow leveraging the [OAuth device authorization grant](https://datatracker.ietf.org/doc/html/rfc8628) standard.

Once you register your app, you need to use the corresponding information to initialize the SDK.
That is done by completing some code in the core authentication script that comes with the SDK, located in `Assets/Auth0/Runtime/AuthManager.cs`.
Once you opened the script, locate the following snippet.

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
Fill the empty `""` with your Auth0 settings. In particular
* `Domain`, `ClientId` and `Scope` are mandatory. The prepopulated values for `Scope` are usually sufficient for most use cases.
* When authentication is performed with the `offline_access` scope included, the application will receive a refresh token that can be used by `AuthManager` to request new tokens on behalf of the user, without forcing the user to perform authentication again. This setting is useful if you want to ensure that your user will not be prompted often, in particular when they close and reopen your Unity application.
* `Audience` is required in case you need an access token to call your own API.


### Handling the Authentication Experience

This version of the Auth0 Unity SDK implements authentication using the same flow featured today by smart devices, such as smart TVs. The smart device displays a URL and a code, whihc the user is invited to enter in another device (such as their phone), where they can go thru the usual authentication experience without the limitations the samrt device would impose. Once authentication succeeds on the second device, the smart device receives the token(s) it needs and authentication takes place. For more details, please refer to [this article](https://auth0.com/device-flow/).
The authentication experience, then, boils down to adding something to your Unity app that can start the token request process in the background, and display to the user a prompt for the URL and code to be entered in the authentication device. The Auth0 Unity SDK offers three alternative methods to do so today.


#### Use a ready-to-use prefab

<p align="center"><img width="400" src="https://user-images.githubusercontent.com/178506/154989639-baabe605-49e6-4e53-82a0-e0999466e4e7.png"></p>

This is by far the easiest method. 
The Auth0 Unity SDK includes a complete authentication dialog in the prefab called *DeviceFlowPrompt*, located in `Assets/Auth0/Prefabs/DeviceFlowPrompt`. All you need to do is to add *DeviceFlowPrompt* in your scene and wire it to your scene elements (eg connect the Canvas to your main camera and the ray  and activate it when you need it.
> Please note: this prefab is meant to be used in the context of VR apps based on the Oculus Integration SDK.

![DeviceFlowPromptInspector](https://user-images.githubusercontent.com/2208132/155464378-1cfd1bea-3d24-447a-a4a3-b7e09e2bde11.png)


#### Use a partially configured prefab

<p align="center"><img width="400" src="https://user-images.githubusercontent.com/178506/151596725-e39b3c70-689f-4d07-803d-906ebfb96f44.png"></p>

The Auth0 Unity SDK includes another prefab, *DeviceFlowRaw* (located in `Assets/Auth0/Prefabs/DeviceFlowRaw`), which provides all the essential elements required to present authenticaiton prompts, but leaves hosting those UX elements and their appearance up to you. 
The *DeviceFlowRaw* doesn't have any dependencies on the Oculus integration SDK and can be used in any Unity application. 


#### Wire up authentication scripts to an UX built from the ground up

Finally, you have the option of skipping prefabs altogether, and wire your experience directly to the authentication logic. Simply add the `Assets/Auth0/Scripts/DeviceFlow` script in your canvas/panel and specify your own UI components:

<p align="center"><img width="500" src="https://user-images.githubusercontent.com/178506/151962282-477f71a5-8aff-47fa-9318-a6347aa25130.png"></p>

* `Instructions`: This canvas contains the instructions to complete the flow, including `Verification Uri` and `User Code` components. This is deactivated by the script when a result (successful or failed) has to be shown to end-user. 
* `Verification Uri`: A text component to set the verification uri returned by Auth0 (usually it looks like `https://{your_auth0_domain}/activate`).
* `User Code`: A text component to set the user code returned by Auth0 (`****-****`).
* `Result`: A text component to show a confirmation message after end-user finished with the flow or an error if something unexpected happens.


### Scenes

The package includes some sample scenes (`Assets/Auth0/Scenes`) you can explore to see the described prefabs in action.

</br>

## More details on AuthManager

The `AuthManager` class is at the core of the Auth0 Unity SDK. It is responsible for wrapping and exposing a selected set of functionality from the underlying Auth0 .NET SDK, including the ability to request tokens via device grant and manage token lifecycle (persistence, logout, etc).
`AuthManager` a singleton instance that exposes the following properties:

* `AuthManager.Instance.Auth0`:
    - Exposes an instance of the [.NET client library v7.15.0 for Auth0 Authentication API](https://auth0.github.io/auth0.net/api/Auth0.AuthenticationApi.AuthenticationApiClient.html#methods).
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
public async void UpdateLoginStatus()
{
    var loggedIn = AuthManager.Instance.Credentials.HasValidCredentials();

    if (loggedIn)
    {
        // Show a welcome message and the SignOut button
        var creds = await AuthManager.Instance.Credentials.GetCredentials();
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
public async void SignOutBtn()
{
    AuthManager.Instance.Credentials.ClearCredentials();
    await this.UpdateLoginStatus();
}
```

</br>

## What is Auth0?

Auth0 helps you to:

* Add authentication with [multiple authentication sources](https://auth0.com/docs/identityproviders), either social like **Google, Facebook, Microsoft Account, LinkedIn, GitHub, Twitter, Box, Salesforce, among others**, or enterprise identity systems like **Windows Azure AD, Google Apps, Active Directory, ADFS or any SAML Identity Provider**.
* Add authentication through more traditional [username/password databases](https://auth0.com/docs/connections/database/custom-db).
* Add support for [linking different user accounts](https://auth0.com/docs/link-accounts) with the same user.
* Support for generating signed [JSON Web Tokens](https://auth0.com/docs/jwt) to call your APIs and **flow the user identity** securely.
* Analytics of how, when, and where users are logging in.
* Pull data from other sources and add it to the user profile, through [JavaScript rules](https://auth0.com/docs/rules/current).

</br>

## Issue Reporting

If you have found a bug or if you have a feature request, please report them at this repository issues section. Please do not report security vulnerabilities on the public GitHub issue tracker. The [Responsible Disclosure Program](https://auth0.com/whitehat) details the procedure for disclosing security issues. This is an Auth0 Lab Experiment, it may not get updated.

</br>

## Author

[Auth0Lab](https://github.com/auth0-lab) - The experimentation arm of [Auth0](https://auth0.com/).

</br>

## License

This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for more info. Auth0Lab experiments should be reviewed with a closer eye than Auth0 repos.
