# Oculus Quest 2 - Device Flow Sample

## Requirements
1. Take a look to the [Auth0 Device Flow's prerequisites](https://auth0.com/docs/quickstart/native/device/01-login#prerequisites) section.
2. Import [Oculus Integration](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022) asset.

## Setup
1. Go to `Assets/Auth0/Runtime/AuthManager.cs` and set `Domain` and `ClientId` settings:

```cs
this.Settings = new Settings
{
    Domain = "", // "acme.auth0.com"
    ClientId = "",
    Scope = "openid profile offline_access",
    Audience = ""
};
```

2. Connect your `Oculus Quest 2` to your computer through a USB cable.
3. Deploy the app from `File -> Build and Run`.

<img width="500" src="https://user-images.githubusercontent.com/178506/151596657-946d3b10-815e-4b82-bd05-ebebde2f34c1.png">
