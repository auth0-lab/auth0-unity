using Auth0.AuthenticationApi.Models;
using System.Threading.Tasks;

namespace Auth0.Api.Credentials
{
    public abstract class BaseCredentialsManager
    {
        public abstract bool HasValidCredentials();

        public abstract void ClearCredentials();

        public abstract Task<Credentials> GetCredentials();

        public abstract void SaveCredentials(AccessTokenResponse tokenResponse, string scope);
    }
}
