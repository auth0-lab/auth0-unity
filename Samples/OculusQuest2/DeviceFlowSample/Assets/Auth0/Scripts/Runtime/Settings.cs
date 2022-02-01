using System;

namespace Auth0
{
    [Serializable]
    public class Settings
    {
        public string Domain;
        public string ClientId;
        public string Audience;
        public string Scope;
    }
}
