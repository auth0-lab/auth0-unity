using Auth0.Api;
using Auth0.Api.Credentials;
using System;

namespace Auth0
{
    /// <summary>
    /// The AuthManager class is at the core of the Auth0 Unity SDK. It is responsible for wrapping and exposing a selected set 
    /// of functionality from the underlying Auth0 .NET SDK, including the ability to request tokens via device grant and manage 
    /// token lifecycle (persistence, logout, etc).
    /// </summary>
    public sealed class AuthManager
    {
        private static readonly Lazy<AuthManager> instance = new Lazy<AuthManager>(() => new AuthManager());

        /// <summary>
        /// Exposes a singleton instance of the <see cref="AuthManager" /> class.
        /// </summary>
        public static AuthManager Instance => instance.Value;

        /// <summary>
        /// Your Auth0 configuration.
        /// </summary>
        public Settings Settings { get; private set; }

        /// <summary>
        /// Exposes an instance of the <see cref="AuthApiClient" /> class.
        /// </summary>
        public AuthApiClient Auth0  { get; private set; }

        /// <summary>
        /// Exposes an instance of an implementation of <see cref="BaseCredentialsManager"/> class.
        /// A utility class to streamline the process of storing and renewing credentials.
        /// </summary>
        public BaseCredentialsManager Credentials  { get; private set; }

        private AuthManager()
        {
            // TODO: use your favorite strategy to load the following settings (ie, RemoteSettings)
            this.Settings = new Settings
            {
                Domain = "", // "acme.auth0.com"
                ClientId = "",
                Scope = "openid profile offline_access",
                Audience = ""
            };

            if (string.IsNullOrWhiteSpace(this.Settings.Domain) || string.IsNullOrWhiteSpace(this.Settings.ClientId) || string.IsNullOrWhiteSpace(this.Settings.Scope))
            {
                throw new InvalidOperationException("'Domain', 'ClientId' and 'Scope' settings are mandatory.");
            }

            this.Auth0 = new AuthApiClient(this.Settings.Domain);

            // IMPORTANT! The PlayerPrefsCredentialsManager will store the Access Token, Refresh Token and ID Token
            // in `/data/data/pkg-name/shared_prefs/pkg-name.v2.playerprefs.xml` as plain-text.
            // Consider implementing a different credentials manager (which inherits from BaseCredentialsManager class)
            // to encrypt those values before persist them.
            this.Credentials = new PlayerPrefsCredentialsManager(this.Auth0, this.Settings.ClientId);
        }
    }
}
