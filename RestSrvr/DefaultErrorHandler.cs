using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using RestSrvr.Exceptions;
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
            response.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(error.ToString()));

            if (error is FileNotFoundException)
                response.StatusCode = 404;
            else if (error is InvalidOperationException)
                response.StatusCode = 500;
            else if (error is XmlException || error is JsonException)
                response.StatusCode = 400;
            else if (error is SecurityException || error is UnauthorizedAccessException)
                response.StatusCode = 401;
            else if (error is NotSupportedException)
                response.StatusCode = 405;
            else if (error is NotImplementedException)
                response.StatusCode = 501;
            else if (error is FaultException)
            {
                response.StatusCode = (error as FaultException).StatusCode;
                if (error.GetType() != typeof(FaultException)) // Special classification
                {
                    var errorData = error.GetType().GetRuntimeProperty("Body").GetValue(error);
                    new DefaultDispatchFormatter().SerializeResponse(response, null, errorData);
                    return true;
                }
            }

            // Load the exception screen
            using (var sr = new StreamReader(typeof(DefaultErrorHandler).Assembly.GetManifestResourceStream("RestSrvr.Resources.ServiceError.html")))
            {
                response.ContentType = "text/html";
                var errRet = sr.ReadToEnd().Replace("{status}", response.StatusCode.ToString()).Replace("{message}", error.Message).Replace("{trace}", error.StackTrace);
                response.Body = new MemoryStream(Encoding.UTF8.GetBytes(errRet));
                response.ContentType = "text/html";
            }
            return true;
        }
    }
}