using Auth0.AuthenticationApi.Models;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System;

namespace Auth0.Api.Credentials
{
    public class Credentials
    {
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
        /// Expiration date of the Access Token.
        /// </summary>
        public DateTime ExpiresAt { get; }

        /// <summary>
        /// Refresh Token that can be used to request new tokens without signing in again.
        /// </summary>
        public string RefreshToken { get; }

        /// <summary>
        /// Identifier Token with user information.
        /// </summary>
        public string IdToken { get; }

        /// <summary>
        /// Access Token's granted scope.
        /// </summary>
        public string Scope { get; }

        /// <summary>
        /// Decoded id_token.
        /// </summary>
        public UserInfo User { get; }
    }
}
