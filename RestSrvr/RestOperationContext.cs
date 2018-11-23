﻿/*
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestSrvr
{
    /// <summary>
    /// Represents the current operation context for the rest service thread
    /// </summary>
    public sealed class RestOperationContext
    {
        // Current reference for thread
        [ThreadStatic]
        private static RestOperationContext m_current;

        // Context
        private HttpListenerContext m_context;

        /// <summary>
        /// Creates a new operation context
        /// </summary>
        internal RestOperationContext(HttpListenerContext context)
        {
            this.m_context = context;
        }

        /// <summary>
        /// Gets the service of the context
        /// </summary>
        public ServiceEndpoint ServiceEndpoint { get; internal set; }

        /// <summary>
        /// Endpoint operation
        /// </summary>
        public EndpointOperation EndpointOperation { get; internal set; }

        /// <summary>
        /// Incoming request
        /// </summary>
        public HttpListenerRequest IncomingRequest => this.m_context.Request;

        /// <summary>
        /// Outgoing resposne
        /// </summary>
        public HttpListenerResponse OutgoingResponse => this.m_context.Response;
        
        /// <summary>
        /// Gets the current operation context
        /// </summary>
        public static RestOperationContext Current
        {
            get { return m_current; }
            internal set { m_current = value; }
        }
    }
}
