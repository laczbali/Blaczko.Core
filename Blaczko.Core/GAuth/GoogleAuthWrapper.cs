using Google.Apis.Auth;

namespace Blaczko.Core.GAuth
{
    /// <summary>
    /// Provides a helper for backend validation of Google Auth tokens <br/>
    /// For getting the token see <see href="https://developers.google.com/identity/sign-in/web/sign-in"/>
    /// </summary>
    public class GoogleAuthWrapper
    {
        private readonly GAuthConfig config;

        public GoogleAuthWrapper(GAuthConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Validates the token and returns the payload if successful.<br/>
        /// Throws a <see cref="InvalidJwtException"/> if the token is invalid.
        /// </summary>
        public async Task<GoogleJsonWebSignature.Payload> Validate(string token)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { config.ClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
            return payload;
        }

        /// <summary>
        /// Returns the users email if the token is valid, returns null if the token is invalid. <br/>
        /// Scope must include "email"
        /// </summary>
        public async Task<string> SimpleValidate(string token)
        {
            try
            {
                var payload = await Validate(token);
                return payload.Email;
            }
            catch (InvalidJwtException)
            {
                return null;
            }
        }
    }
}
