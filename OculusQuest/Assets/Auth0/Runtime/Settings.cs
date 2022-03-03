using System;

namespace Auth0
{
    /// <summary>
    /// Represents your Auth0 configuration.
    /// </summary>
    [Serializable]
    public class Settings
    {
        /// <summary>
        /// Your Auth0 domain name, e.g. tenant.auth0.com.
        /// </summary>
        public string Domain;
        
        /// <summary>
        /// Your Auth0 client id.
        /// </summary>
        public string ClientId;
        
        /// <summary>
        /// The unique identifier of your API. Audience is required in case you need an access token to call your own API.
        /// </summary>
        public string Audience;
        
        /// <summary>
        /// The scopes for which you want to request authorization. These must be separated by a space, e.g. openid profile offline_access.
        /// </summary>
        /// <remarks>
        /// When authentication is performed with the "offline_access" scope included, the application will receive a refresh token that can be
        /// used by AuthManager to request new tokens on behalf of the user, without forcing the user to perform authentication again.
        /// This setting is useful if you want to ensure that your user will not be prompted often, in particular when they close and reopen your Unity application.
        /// </remarks>
        public string Scope;
    }
}
