using Auth0.AuthenticationApi.Models;
using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;

namespace Auth0.Api.Credentials
{
    public class PlayerPrefsCredentialsManager : BaseCredentialsManager
    {
        private const string KEY_ACCESS_TOKEN = "com.auth0.access_token";
        private const string KEY_REFRESH_TOKEN = "com.auth0.refresh_token";
        private const string KEY_ID_TOKEN = "com.auth0.id_token";
        private const string KEY_EXPIRES_AT = "com.auth0.expires_at";
        private const string KEY_SCOPE = "com.auth0.scope";
        
        private readonly AuthApiClient auth0;

        private readonly string clientId;

        public PlayerPrefsCredentialsManager(AuthApiClient authApiClient, string clientId)
        {
            this.auth0 = authApiClient;
            this.clientId = clientId;
        }

        public override bool HasValidCredentials()
        {
            var accessToken = PlayerPrefs.GetString(KEY_ACCESS_TOKEN);
            var refreshToken = PlayerPrefs.GetString(KEY_REFRESH_TOKEN);
            var idToken = PlayerPrefs.GetString(KEY_ID_TOKEN);
            var expiresAt = PlayerPrefs.GetString(KEY_EXPIRES_AT);

            var emptyCredentials = string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(expiresAt);
            if (emptyCredentials)
            {
                return false;
            }

            var accessTokenExpired = DateTime.Parse(expiresAt, CultureInfo.InvariantCulture).CompareTo(DateTime.UtcNow) <= 0;
            if (accessTokenExpired && string.IsNullOrWhiteSpace(refreshToken))
            {
                return false;
            }

            return true;
        }

        public override void ClearCredentials()
        {
            PlayerPrefs.DeleteKey(KEY_ACCESS_TOKEN);
            PlayerPrefs.DeleteKey(KEY_REFRESH_TOKEN);
            PlayerPrefs.DeleteKey(KEY_ID_TOKEN);
            PlayerPrefs.DeleteKey(KEY_EXPIRES_AT);
            PlayerPrefs.DeleteKey(KEY_SCOPE);
        }

        public override async Task<Credentials> GetCredentials()
        {
            var accessToken = PlayerPrefs.GetString(KEY_ACCESS_TOKEN);
            var refreshToken = PlayerPrefs.GetString(KEY_REFRESH_TOKEN);
            var idToken = PlayerPrefs.GetString(KEY_ID_TOKEN);
            var expiresAtStr = PlayerPrefs.GetString(KEY_EXPIRES_AT);
            var scope = PlayerPrefs.GetString(KEY_SCOPE);

            var emptyCredentials = string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(expiresAtStr);
            if (emptyCredentials)
            {
                throw new InvalidOperationException("No Credentials were previously set.");
            }

            var expiresAt = DateTime.Parse(expiresAtStr, CultureInfo.InvariantCulture);
            var accessTokenExpired = expiresAt.CompareTo(DateTime.UtcNow) <= 0;
            if (!accessTokenExpired) {
                return new Credentials
                {
                    AccessToken = accessToken,
                    ExpiresAt = expiresAt,
                    RefreshToken = refreshToken,
                    IdToken = idToken,
                    Scope = scope
                };
            }

            // Access token is expired, use refresh token to request a new one.
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new InvalidOperationException("Credentials need to be renewed but no Refresh Token is available to renew them.");
            }

            try
            {
                var tokenResp = await this.auth0.GetTokenAsync(new RefreshTokenRequest
                {
                    ClientId = this.clientId,
                    RefreshToken = refreshToken
                });
                var renewedCredentials = tokenResp.ToCredentials(scope);

                SaveCredentials(renewedCredentials);
                return renewedCredentials;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while trying to use the Refresh Token to renew the Credentials.", ex);
            }
        }

        public override void SaveCredentials(AccessTokenResponse tokenResponse, string scope)
        {
            SaveCredentials(tokenResponse.ToCredentials(scope));
        }

        private static void SaveCredentials(Credentials credentials)
        {
            PlayerPrefs.SetString(KEY_ACCESS_TOKEN, credentials.AccessToken);
            PlayerPrefs.SetString(KEY_REFRESH_TOKEN, credentials.RefreshToken);
            PlayerPrefs.SetString(KEY_ID_TOKEN, credentials.IdToken);
            PlayerPrefs.SetString(KEY_EXPIRES_AT, credentials.ExpiresAt.ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.SetString(KEY_SCOPE, credentials.Scope);
        }
    }

    public static class CredentialsExtensions
    {
        public static Credentials ToCredentials(this AccessTokenResponse tokenResponse, string scope)
        {
            return new Credentials
            {
                AccessToken = tokenResponse.AccessToken,
                ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                RefreshToken = tokenResponse.RefreshToken,
                IdToken = tokenResponse.IdToken,
                Scope = scope,
            };
        }
    }
}