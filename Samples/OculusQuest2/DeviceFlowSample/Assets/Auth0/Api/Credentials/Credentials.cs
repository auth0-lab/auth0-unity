using Newtonsoft.Json;
using System;

namespace Auth0.Api.Credentials
{
    public class Credentials
    {
        /// <summary>
        /// Access token for specified API (audience).
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Expiration date of the Access Token.
        /// </summary>
        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Refresh Token that can be used to request new tokens without signing in again.
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Identifier Token with user information.
        /// </summary>
        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        /// <summary>
        /// Access Token's granted scope.
        /// </summary>
        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}
