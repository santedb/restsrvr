/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
 *
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
 * User: justin
 * Date: 2018-11-19
 */
using System;

namespace RestSrvr.Exceptions
{
    /// <summary>
    /// Represents a fault
    /// </summary>
    public class FaultException : Exception
    {

        /// <summary>
        /// Creates a new fault
        /// </summary>
        public FaultException(Int32 statusCode) : this(statusCode, null, null)
        {
        }

        /// <summary>
        /// Creates a new fault
        /// </summary>
        public FaultException(Int32 statusCode, String message) : this(statusCode, message, null)
        {
        }

        /// <summary>
        /// Creates a new fault
        /// </summary>
        public FaultException(Int32 statusCode, String message, Exception innerException) : base(message, innerException)
        {
            this.StatusCode = statusCode;
        }
        /// <summary>
        /// Status code to supply
        /// </summary>
        public Int32 StatusCode { get; set; }

    }
    /// <summary>
    /// Represents a fault with a type of response body to be sent
    /// </summary>
    public class FaultException<TBody> : FaultException
    {

       
        /// <summary>
        /// The body object of the fault
        /// </summary>
        public TBody Body { get; set; }

        /// <summary>
        /// Represents the fault exception
        /// </summary>
        public FaultException(Int32 statusCode, TBody data) : this(statusCode, data, null, null)
        {
        }

        /// <summary>
        /// Represents the fault exception
        /// </summary>
        public FaultException(Int32 statusCode, TBody data, String message) : this(statusCode, data, message, null)
        {
        }

        /// <summary>
        /// Represents the fault exception
        /// </summary>
        public FaultException(Int32 statusCode, TBody data, String message, Exception innerException) : base(statusCode, message, innerException)
        {
            this.Body = data;
        }


    }
}
