using Auth0.Api.Tokens;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Auth0.Api
{
    public class AuthApiClient : AuthenticationApiClient {
        private static readonly IdTokenValidator idTokenValidator = new IdTokenValidator();
        private readonly TimeSpan idTokenValidationLeeway = TimeSpan.FromMinutes(1);

        public AuthApiClient(string domain) : base(new Uri(String.Format("https://{0}", domain)))
        {
        }

        public async Task<AccessTokenResponse> ExchangeDeviceCodeAsync(string clientId, string deviceCode, int retryInterval, CancellationToken cancellationToken = default)
        {
            AccessTokenResponse response = null;
            ErrorApiException apiError;

            var request = new DeviceCodeTokenRequest
            {
                ClientId = clientId,
                DeviceCode = deviceCode
            };

            do
            {
                await Task.Delay(TimeSpan.FromSeconds(retryInterval));
                apiError = null;

                try
                {
                    response = await this.GetTokenAsync(request, cancellationToken);
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
                // See https://github.com/auth0/auth0.net/blob/8973a9d0962c359e38543eea02b8feeef809651d/src/Auth0.AuthenticationApi/AuthenticationApiClient.cs#L308
                await AssertIdTokenValid(response.IdToken, clientId, JwtSignatureAlgorithm.RS256, string.Empty).ConfigureAwait(false);
            }

            return response;
        }

        private Task AssertIdTokenValid(string idToken, string audience, JwtSignatureAlgorithm algorithm, string clientSecret, string organization = null)
        {
            var requirements = new IdTokenRequirements(algorithm, this.BaseUri.AbsoluteUri, audience, idTokenValidationLeeway, null, organization);
            return idTokenValidator.Assert(requirements, idToken, clientSecret);
        }
    }
}
