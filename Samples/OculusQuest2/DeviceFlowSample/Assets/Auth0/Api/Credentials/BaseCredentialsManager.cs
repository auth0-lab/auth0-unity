using System.Threading.Tasks;

namespace Auth0.Api.Credentials
{
    public abstract class BaseCredentialsManager
    {
        public abstract bool HasValidCredentials();

        public abstract void ClearCredentials();

        public abstract Task<Credentials> GetCredentials();

        public abstract void SaveCredentials(Credentials credentials);
    }
}
