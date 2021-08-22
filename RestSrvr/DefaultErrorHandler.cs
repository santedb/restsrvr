/*
 * Copyright (C) 2021 - 2021, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors
 * Portions Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2021-8-5
 */
using Newtonsoft.Json;
using RestSrvr.Exceptions;
using RestSrvr.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Xml;

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

            if (error is FileNotFoundException || error is KeyNotFoundException)
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
            else
                response.StatusCode = 500;

            // Load the exception screen
            using (var sr = new StreamReader(typeof(DefaultErrorHandler).Assembly.GetManifestResourceStream("RestSrvr.Resources.ServiceError.html")))
            {
                response.ContentType = "text/html";
                var errRet = sr.ReadToEnd().Replace("{status}", response.StatusCode.ToString())
                    .Replace("{description}", response.StatusDescription)
                    .Replace("{type}", error.GetType().Name)
                    .Replace("{message}", error.Message)
                    .Replace("{details}", error.ToString())
                    .Replace("{trace}", error.StackTrace);
                response.Body = new MemoryStream(Encoding.UTF8.GetBytes(errRet));
                response.ContentType = "text/html";
            }
            return true;
        }
    }
}