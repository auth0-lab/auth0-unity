# auth0-unity-sdk
An experimental Auth0 SDK for Unity platform.

## Requirements

* Take a look to the [Auth0 Device Flow's prerequisites](https://auth0.com/docs/quickstart/native/device/01-login#prerequisites) section.
* Unity 2020.x (or later)

## Installation

* **Option 1 _(not available yet)_:** Install and import the Auth0 package from [Unity Assets Store](https://assetstore.unity.com/).
* **Option 2:** Download [Auth0UnitySDK.unitypackage](https://github.com/auth0-lab/auth0-unity-sdk/raw/main/Auth0UnitySDK-v0.0.1.unitypackage) and import it in your proyect as a `Custom Package`.

<img width="500" src="https://user-images.githubusercontent.com/178506/151574518-1a5bad47-cb07-433d-998a-5e1398b8f181.png">

## Usage

The first thing to do is to initialize the `AuthManager` script. It expose a singletone instance with the following properties:

* `AuthManager.Instance.Auth0`: Auth0 Authentication API client.
* `AuthManager.Instance.Credentials`: A utility class to streamline the process of storing and renewing credentials. You can access the `AccessToken` or `IdToken` properties from the `Credentials` instance.

The Auth0 SDK provides an empty scene to initialize the `AuthManager` script for you before the rest of your components. Just include the `Assets/Auth0/Scenes/Preload` scene to your project and set it as the first project's scene in `File -> Build Settings... -> Scenes In Build` section:

<img width="500" src="https://user-images.githubusercontent.com/178506/151579578-d28f7698-0bb5-4075-b2b5-b441a238ae44.png">

Then, set your Auth0 `Domain` and `Client Id` in the `AuthManager` properties, located in the `Inspector` tab:

<img width="500" src="https://user-images.githubusercontent.com/178506/151585295-818fa303-41e2-4e21-93b9-081717c0f91d.png">

Finally, include the `Assets/Auth0/Prefabs/DeviceFlow` prefab in the scene/section that you consider, and (optionally) set the following properties:

<img width="500" src="https://user-images.githubusercontent.com/178506/151587301-ea28bc25-6a7e-44eb-904b-ad4b329b3227.png">

* When authentication is performed with the `offline_access` scope included, it returns a refresh token that can be used by `AuthManager` to request a new user token, without forcing the user to perform authentication again.
* If you don't want to use default UI components to show verification uri, user code and result, just override them from `UI Components` section.

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

<img width="400" src="https://user-images.githubusercontent.com/178506/151596657-946d3b10-815e-4b82-bd05-ebebde2f34c1.png"> <img width="400" src="https://user-images.githubusercontent.com/178506/151596725-e39b3c70-689f-4d07-803d-906ebfb96f44.png">


