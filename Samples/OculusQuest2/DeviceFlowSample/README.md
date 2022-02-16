# Oculus Quest 2 - Device Flow Sample

## Requirements
1. Take a look to the [Auth0 Device Flow's prerequisites](https://auth0.com/docs/quickstart/native/device/01-login#prerequisites) section.
2. Unity Editor 2020.3.26f1
3. Import [Oculus Integration v37](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022) from Unity Asset Store.

## Setup
1. Make sure that `Scenes/MainMenu` is included in the build (`File -> Build Settings...` under `Scenes In Build` section).
2. Go to `Assets/Auth0/Runtime/AuthManager.cs` and set `Domain` and `ClientId` settings:

```cs
this.Settings = new Settings
{
    Domain = "", // "acme.auth0.com"
    ClientId = "",
    Scope = "openid profile offline_access",
    Audience = "" // leave it empty unless you need an access token to make calls to your API
};
```

3. Connect your `Oculus Quest 2` to your computer through a USB cable.
4. Deploy the app from `File -> Build and Run`.

<img width="500" src="https://user-images.githubusercontent.com/178506/151596657-946d3b10-815e-4b82-bd05-ebebde2f34c1.png">
