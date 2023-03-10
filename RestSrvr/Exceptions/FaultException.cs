/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-3-10
 */
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace RestSrvr.Exceptions
{
    /// <summary>
    /// Represents a fault
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FaultException : Exception
    {
        /// <summary>
        /// Creates a new fault
        /// </summary>
        public FaultException(HttpStatusCode statusCode) : this(statusCode, null, null)
        {

        }

        /// <summary>
        /// Creates a new fault
        /// </summary>
        public FaultException(HttpStatusCode statusCode, String message, Object resultData = null) : this(statusCode, message, null)
        {
            if (resultData != null)
            {
                this.Data.Add("result", resultData);
            }
        }

        /// <summary>
        /// Creates a new fault
        /// </summary>
        public FaultException(HttpStatusCode statusCode, String message, Exception innerException) : base(message, innerException)
        {
            this.StatusCode = statusCode;
            this.Headers = new WebHeaderCollection();
        }

        /// <summary>
        /// Status code to supply
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets the body of the fault
        /// </summary>
        public object Body { get; set; }
        /// <summary>
        /// Get the headers to add to the response
        /// </summary>
        public WebHeaderCollection Headers { get; }
    }

    /// <summary>
    /// Represents a fault with a type of response body to be sent
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FaultException<TBody> : FaultException
    {
        /// <summary>
        /// The body object of the fault
        /// </summary>
        public new TBody Body { get => (TBody)base.Body; set => base.Body = value; }

        /// <summary>
        /// Represents the fault exception
        /// </summary>
        public FaultException(HttpStatusCode statusCode, TBody data) : this(statusCode, data, null, null)
        {
        }

        /// <summary>
        /// Represents the fault exception
        /// </summary>
        public FaultException(HttpStatusCode statusCode, TBody data, String message) : this(statusCode, data, message, null)
        {
        }

        /// <summary>
        /// Represents the fault exception
        /// </summary>
        public FaultException(HttpStatusCode statusCode, TBody data, String message, Exception innerException) : base(statusCode, message, innerException)
        {
            this.Body = data;
        }
    }
}