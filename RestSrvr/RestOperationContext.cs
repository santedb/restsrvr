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
using System;
using System.Collections.Generic;
using System.Net;

namespace RestSrvr
{
    /// <summary>
    /// Represents the current operation context for the rest service thread
    /// </summary>
    public sealed class RestOperationContext : IDisposable
    {
        // Current reference for thread
        [ThreadStatic]
        private static RestOperationContext m_current;

        // Context
        private HttpListenerContext m_context;
        // Applied policies
        private List<IServicePolicy> m_appliedPolicies = new List<IServicePolicy>();

        /// <summary>
        /// Fired when the object is disposed
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Creates a new operation context
        /// </summary>
        internal RestOperationContext(HttpListenerContext context)
        {
            this.m_context = context;
            this.Data = new Dictionary<String, Object>()
            {
                { "uuid", Guid.NewGuid() }
            };
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

        /// <summary>
        /// Policies that were applied on the context
        /// </summary>
        public IEnumerable<IServicePolicy> AppliedPolicies => this.m_appliedPolicies.AsReadOnly();

        /// <summary>
        /// A series of data element associated with the operation context
        /// </summary>
        public IDictionary<String, Object> Data { get; }

        /// <summary>
        /// Close the context 
        /// </summary>
        public void Dispose()
        {
            this.m_context.Response.Close();
            this.Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Add an applied policy
        /// </summary>
        internal void AddAppliedPolicy(IServicePolicy pol)
        {
            this.m_appliedPolicies.Add(pol);
        }
    }
}
