using Auth0.AuthenticationApi;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace Auth0.Api.Tokens
{
    internal class AsymmetricSignedDecoder : SignedDecoder
    {
        public AsymmetricSignedDecoder(IList<JsonWebKey> keys)
            : base(JwtSignatureAlgorithm.RS256, keys)
        {
        }
    }
}
