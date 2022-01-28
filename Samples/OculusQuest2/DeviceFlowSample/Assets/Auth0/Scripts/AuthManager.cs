using Auth0.Api;
using Auth0.Api.Credentials;
using System;
using UnityEngine;

namespace Auth0
{
    public class AuthManager : MonoBehaviour
    {
        [Header("Auth0")]
        public string Domain;
        public string ClientId;
        
        public static AuthManager Instance { get; private set; }

        public AuthApiClient auth0;

        public BaseCredentialsManager credentials;

        private void Awake()
        {
            if (string.IsNullOrWhiteSpace(this.Domain) || string.IsNullOrWhiteSpace(this.ClientId))
            {
                throw new ArgumentNullException("Missing Configuration", "Please go to 'Preload' scene and set 'Auth0 Domain' and 'Auth0 Client Id' properties under 'Auth Manager' script section.");
            }

            this.auth0 = new AuthApiClient(this.Domain, this.ClientId);

            // IMPORTANT! The PlayerPrefsCredentialsManager will store the Access Token, Refresh Token and ID Token
            // in `/data/data/pkg-name/shared_prefs/pkg-name.v2.playerprefs.xml` as plain-text.
            // Consider implementing a different credentials manager (which inherits from BaseCredentialsManager class)
            // to encrypt those values before persist them.
            this.credentials = new PlayerPrefsCredentialsManager(this.auth0);

            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
}
