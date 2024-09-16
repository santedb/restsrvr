/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 */
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace RestSrvr.Description
{
    /// <summary>
    /// Describes a single endpoint
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class EndpointDescription
    {
        // Trace source
        private TraceSource m_traceSource = new TraceSource(TraceSources.DescriptionTraceSourceName);

        private ContractDescription m_contract;
        private String m_rawUrl;
        private Uri m_listenUri;

        /// <summary>
        /// Creates a new endpoint description with the specified base URI and contract
        /// </summary>
        public EndpointDescription(Uri baseUri, ContractDescription contract)
        {
            this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Enter EndpointDescription CTOR ({0}, {1})", baseUri, contract);
            this.m_contract = contract;

            if (baseUri.Host == "0.0.0.0")
            {
                this.m_rawUrl = baseUri.ToString().Replace("://0.0.0.0", "://+");
            }
            else
            {
                this.m_rawUrl = baseUri.ToString();
            }

            if (!this.m_rawUrl.EndsWith("/"))
            {
                this.m_rawUrl += "/";
            }

            this.m_listenUri = baseUri;
        }

        /// <summary>
        /// Create an endpoint description from the specified base URI and behavior
        /// </summary>
        /// <param name="baseUri">The base URI for listening</param>
        /// <param name="contractType">The contract type</param>
        public EndpointDescription(Uri baseUri, Type contractType) : this(baseUri, new ContractDescription(contractType))
        {
        }

        /// <summary>
        /// Gets the contract that this endpoint implements / uses
        /// </summary>
        public ContractDescription Contract => this.m_contract;

        /// <summary>
        /// The listening URI for the endpoint
        /// </summary>
        public Uri ListenUri => this.m_listenUri;

        /// <summary>
        /// The listening URI for the endpoint
        /// </summary>
        public String RawUrl => this.m_rawUrl;
    }
}
