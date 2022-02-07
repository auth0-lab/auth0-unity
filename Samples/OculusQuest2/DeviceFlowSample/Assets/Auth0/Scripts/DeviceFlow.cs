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
    public class DeviceFlow : MonoBehaviour
    {
        [Header("UI Components")]
        public Canvas Instructions;
        public Text VerificationUri;
        public Text UserCode;
        public Text Result;

        private async void OnEnable()
        {
            await this.StartFlow();
        }

        private async Task StartFlow()
        {
            try
            {
                this.ResetInstructions();

                var auth0 = AuthManager.Instance.Auth0;
                var clientId = AuthManager.Instance.Settings.ClientId;
                var scope = AuthManager.Instance.Settings.Scope;
                var audience = AuthManager.Instance.Settings.Audience;
                var deviceCodeResp = await auth0.StartDeviceFlowAsync(new DeviceCodeRequest
                {
                    ClientId = clientId,
                    Scope = scope,
                    Audience = audience
                });

                this.VerificationUri.text = deviceCodeResp.VerificationUri;
                this.UserCode.text = deviceCodeResp.UserCode;

                var tokenResp = await auth0.ExchangeDeviceCodeAsync(clientId, deviceCodeResp.DeviceCode, deviceCodeResp.Interval);
                AuthManager.Instance.Credentials.SaveCredentials(tokenResp, scope);

                var callUserInfo = scope.Split(' ').Any("openid".Contains);
                var userInfo = callUserInfo ? await auth0.GetUserInfoAsync(tokenResp.AccessToken) : null;
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
