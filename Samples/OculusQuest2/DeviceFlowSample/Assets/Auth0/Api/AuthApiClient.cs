using Auth0.Api.Tokens;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Auth0.Api
{
    public class AuthApiClient {
        private readonly string clientId;
        private readonly AuthenticationApiClient client;
        private static readonly IdTokenValidator idTokenValidator = new IdTokenValidator();
        private readonly TimeSpan idTokenValidationLeeway = TimeSpan.FromMinutes(1);

        public AuthApiClient(string domain, string clientId)
        {
            this.clientId = clientId;
            this.client = new AuthenticationApiClient(new Uri(String.Format("https://{0}", domain)));
        }

        public Task<DeviceCodeResponse> StartDeviceFlow(string scope = "openid profile offline_access", string audience = "")
        {
            return this.client.StartDeviceFlowAsync(new DeviceCodeRequest
            {
                ClientId = this.clientId,
                Scope = scope,
                Audience = audience
            });
        }

        public async Task<AccessTokenResponse> ExchangeDeviceCode(string deviceCode, int retryInterval)
        {
            AccessTokenResponse response = null;
            ErrorApiException apiError;

            var request = new DeviceCodeTokenRequest
            {
                ClientId = this.clientId,
                DeviceCode = deviceCode
            };

            do
            {
                await Task.Delay(TimeSpan.FromSeconds(retryInterval));
                apiError = null;

                try
                {
                    response = await this.client.GetTokenAsync(request);
                }
                catch (ErrorApiException ex) 
                {
                    apiError = ex;
                }
            } while((apiError != null && apiError.ApiError.ErrorCode != "authorization_pending") || response == null);
            
            if (apiError != null)
            {
                throw apiError;    
            }

            if (!string.IsNullOrWhiteSpace(response.IdToken))
            {
                // GetTokenAsync(DeviceCodeTokenRequest) isn't validating the id_token
                await AssertIdTokenValid(response.IdToken, this.clientId, JwtSignatureAlgorithm.RS256, string.Empty).ConfigureAwait(false);
            }

            return response;
        }

        public Task<AccessTokenResponse> ExchangeRefreshToken(string refreshToken)
        {
            // GetTokenAsync(RefreshTokenRequest) is already validating the id_token
            return this.client.GetTokenAsync(new RefreshTokenRequest
            {
                ClientId = this.clientId,
                RefreshToken = refreshToken
            });
        }

        public Task<UserInfo> GetUserInfo(string accessToken)
        {
            return this.client.GetUserInfoAsync(accessToken);
        } 

        private Task AssertIdTokenValid(string idToken, string audience, JwtSignatureAlgorithm algorithm, string clientSecret, string organization = null)
        {
            var requirements = new IdTokenRequirements(algorithm, this.client.BaseUri.AbsoluteUri, audience, idTokenValidationLeeway, null, organization);
            return idTokenValidator.Assert(requirements, idToken, clientSecret);
        }
    }
}
