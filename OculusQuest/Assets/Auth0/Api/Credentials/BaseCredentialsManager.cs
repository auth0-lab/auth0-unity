using Auth0.AuthenticationApi.Models;
using System.Threading.Tasks;

namespace Auth0.Api.Credentials
{
    /// <summary>
    /// Base class meant to abstract common logic across Credentials Manager implementations.
    /// </summary>
    public abstract class BaseCredentialsManager
    {
        /// <summary>
        /// Checks if a non-expired pair of credentials can be obtained from this manager.
        /// </summary>
        /// <returns>Whether there are valid credentials stored on this manager.</returns>
        public abstract bool HasValidCredentials();

        /// <summary>
        /// Removes the credentials from the storage if present.
        /// </summary>
        public abstract void ClearCredentials();

        /// <summary>
        /// Retrieves the credentials from the storage and refresh them if they have already expired.
        /// </summary>
        /// <returns><see cref="Task"/> representing the async operation containing a valid <see cref="Credentials" />.</returns>
        public abstract Task<Credentials> GetCredentials();
        
        /// <summary>
        /// Stores the given credentials in the storage.
        /// </summary>
        /// <param name="tokenResponse"><see cref="AccessTokenResponse"/> containing valid tokens.</param>
        /// <param name="scope">The scope used to request for the access token.</param>
        public abstract void SaveCredentials(AccessTokenResponse tokenResponse, string scope);
    }
}
