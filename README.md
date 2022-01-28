# auth0-unity-sdk
An experimental Auth0 SDK for Unity platform.

## Requirements

* Take a look to the [Auth0 Device Flow's prerequisites](https://auth0.com/docs/quickstart/native/device/01-login#prerequisites) section.
* TBC

## Installation

* **Option 1 _(not available yet)_:** Install and import the Auth0 package from [Unity Assets Store](https://assetstore.unity.com/)
* **Option 2:** Download [Auth0UnitySDK.unitypackage](TBC) and import it in your proyect as a `Custom Package`.

<img width="500" src="https://user-images.githubusercontent.com/178506/151574518-1a5bad47-cb07-433d-998a-5e1398b8f181.png">

## Usage

The first thing to do is to initialize the `AuthManager` script. It expose a singletone instance with the following properties:

* `AuthManager.Instance.Auth0`: Auth0 Authentication API client.
* `AuthManager.Instance.Credentials`: A utility class to streamline the process of storing and renewing credentials. You can access the `AccessToken` or `IdToken` properties from the `Credentials` instance:
  - `bool HasValidCredentials()`: Stored credentials are considered valid if they have not expired or can be refreshed. Useful to check if a user has already logged in.
  - `void ClearCredentials()`: Remove the stored credentials. Useful to log the user out of your app.
  - `Task<Credentials> GetCredentials()`: If the access token has expired, the credentials manager automatically uses the refresh token and renews the credentials. New credentials are be stored for future access.
  - `SaveCredentials(Credentials)`: Save the credentials (obtained, for example, during device flow).

The Auth0 SDK provides an empty scene to initialize the `AuthManager` script before the rest of the components. Just include the `Auth0/Scenes/Preload` scene to your project and make sure to open it before any other scene. From `File -> Build Settings...`:

<img width="500" src="https://user-images.githubusercontent.com/178506/151579578-d28f7698-0bb5-4075-b2b5-b441a238ae44.png">
