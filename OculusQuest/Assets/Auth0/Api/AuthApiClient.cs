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
    /// <summary>
    /// Client for the Auth0 Authentication API.
    /// </summary>
    /// <remarks>
    /// Full documentation on the Auth0 Authentication API is available at https://auth0.com/docs/api/authentication
    /// </remarks>
    public class AuthApiClient : AuthenticationApiClient {
        private static readonly IdTokenValidator idTokenValidator = new IdTokenValidator();
        private readonly TimeSpan idTokenValidationLeeway = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthApiClient" /> class.
        /// </summary>
        /// <param name="domain">Your Auth0 domain name, e.g. tenant.auth0.com.</param>
        public AuthApiClient(string domain) : base(new Uri(String.Format("https://{0}", domain)))
        {
        }

        /// <summary>
        /// A helper methods to poll your Auth0 Authorization Server for an access token using the time period specified by retryInterval.
        /// The method continues polling until either the user completes the browser flow path, the user code expires or an unexpected error is returned by Auth0.
        /// </summary>
        /// <param name="clientId">Your Auth0 client id.</param>
        /// <param name="deviceCode">Device code to be exchanged.</param>
        /// <param name="retryInterval">The interval (in seconds) to poll the token URL to request a token.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns><see cref="Task"/> representing the async operation containing a valid <see cref="AccessTokenResponse" /> with the requested tokens.</returns>
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
