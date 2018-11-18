using System;
using System.IO;
using RestSrvr.Message;

namespace RestSrvr
{
    /// <summary>
    /// Represents the default error handler
    /// </summary>
    internal class DefaultErrorHandler : IServiceErrorHandler
    {
        /// <summary>
        /// Handle error
        /// </summary>
        public bool HandleError(Exception error)
        {
            return true;
        }

        /// <summary>
        /// Provide the fault to the pipeline
        /// </summary>
        public bool ProvideFault(Exception error, RestResponseMessage response)
        {
            response.ContentType = "text/plain";
            response.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(error.ToString()));
            response.StatusCode = 500;
            return true;
        }
    }
}