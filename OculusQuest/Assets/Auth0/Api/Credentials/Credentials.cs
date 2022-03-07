using Auth0.AuthenticationApi.Models;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Auth0.Api.Credentials
{
    /// <summary>
    /// Holds the user's credentials returned by Auth0.
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Credentials" /> class.
        /// </summary>
        /// <param name="accessToken">Access token for specified API (audience).</param>
        /// <param name="expiresAt">Expiration date of the access token.</param>
        /// <param name="refreshToken">Refresh Token that can be used to request new tokens without signing in again.</param>
        /// <param name="idToken">Identifier token with user information.</param>
        /// <param name="scope">Access token's granted scope.</param>
        public Credentials(string accessToken, DateTime expiresAt, string refreshToken, string idToken, string scope)
        {
            this.AccessToken = accessToken;
            this.ExpiresAt = expiresAt;
            this.RefreshToken = refreshToken;
            this.IdToken = idToken;
            this.Scope = scope;

            if (!string.IsNullOrWhiteSpace(this.IdToken))
            {
                // id_token is already validated
                var jwtHandler = new JwtSecurityTokenHandler();
                var decodedIdToken = jwtHandler.ReadJwtToken(this.IdToken);
                this.User = JsonConvert.DeserializeObject<UserInfo>(decodedIdToken.Payload.SerializeToJson());
            }
        }

        /// <summary>
        /// Access token for specified API (audience).
        /// </summary>
        public string AccessToken { get; }

        /// <summary>
        /// Expiration date of the access token.
        /// </summary>
        public DateTime ExpiresAt { get; }

        /// <summary>
        /// Refresh Token that can be used to request new tokens without signing in again.
        /// </summary>
        public string RefreshToken { get; }

        /// <summary>
        /// Identifier token with user information.
        /// </summary>
        public string IdToken { get; }

        /// <summary>
        /// Access token's granted scope.
        /// </summary>
        public string Scope { get; }

        /// <summary>
        /// Decoded IdToken.
        /// </summary>
        public UserInfo User { get; }
    }
}
