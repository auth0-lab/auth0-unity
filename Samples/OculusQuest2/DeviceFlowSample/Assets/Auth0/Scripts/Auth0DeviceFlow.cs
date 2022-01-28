using Auth0.Api;
using Auth0.Api.Credentials;
using Auth0.AuthenticationApi.Models;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Auth0
{
    public class Auth0DeviceFlow : MonoBehaviour
    {
        [Header("Auth Params")]
        public string Scope = "openid profile offline_access";
        public string Audience;
        
        [Header("UI Components")]
        public Canvas Instructions;
        public Text VerificationUri;
        public Text UserCode;
        public Text Result;

        private async void OnEnable()
        {
            await this.DeviceFlow();
        }

        private async Task DeviceFlow()
        {
            try
            {
                this.ResetInstructions();

                var auth0 = AuthManager.Instance.auth0;
                var deviceCodeResp = await auth0.StartDeviceFlow(this.Scope, this.Audience);

                this.VerificationUri.text = deviceCodeResp.VerificationUri;
                this.UserCode.text = deviceCodeResp.UserCode;

                var tokenResp = await auth0.ExchangeDeviceCode(deviceCodeResp.DeviceCode, deviceCodeResp.Interval);

                AuthManager.Instance.credentials.SaveCredentials(new Credentials
                {
                    AccessToken = tokenResp.AccessToken,
                    ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResp.ExpiresIn),
                    RefreshToken = tokenResp.RefreshToken,
                    IdToken = tokenResp.IdToken,
                    Scope = this.Scope,
                });

                var callUserInfo = this.Scope.Split(' ').Any("openid".Contains);
                var userInfo = callUserInfo ? await auth0.GetUserInfo(tokenResp.AccessToken) : null;
                if (userInfo != null && !String.IsNullOrEmpty(userInfo.FullName)) {
                    this.ShowResult(String.Format("Hello {0}, you're all set!", userInfo.FullName));
                } else {
                    this.ShowResult("You're all set!");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                this.ShowResult("An unexpected error has occurred. Please try again, and if the problem persists, contact support for further assistance.");
            }
        }

        private void Awake()
        {
            if (this.Instructions == null)
            {
                this.Instructions = GameObject.Find("Instructions").GetComponent<Canvas>();
            }

            if (this.VerificationUri == null)
            {
                this.VerificationUri = GameObject.Find("VerificationUri").GetComponent<Text>();
            }

            if (this.UserCode == null)
            {
                this.UserCode = GameObject.Find("UserCode").GetComponent<Text>();
            }

            if (this.Result == null)
            {
                this.Result = GameObject.Find("Result").GetComponent<Text>();
            }
        }

        private void ResetInstructions()
        {
            this.VerificationUri.text = "...";
            this.UserCode.text = "...";
            this.Result.gameObject.SetActive(false);
            this.Instructions.gameObject.SetActive(true);
        }

        private void ShowResult(string message)
        {
            this.Result.text = message;
            this.Instructions.gameObject.SetActive(false);
            this.Result.gameObject.SetActive(true);
        }
    }
}
