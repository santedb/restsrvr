using RestSrvr.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestSrvr
{
    /// <summary>
    /// Service behavior which authorizes users
    /// </summary>
    public interface IAuthorizationServicePolicy : IServicePolicy
    {

        /// <summary>
        /// Add the appropriate authentication headers to the response
        /// </summary>
        /// <param name="faultMessage">The fault message where headers should be added</param>
        /// <param name="error">The error code that caused the challenge header to be added</param>
        void AddAuthenticateChallengeHeader(RestResponseMessage faultMessage, Exception error);

    }
}
