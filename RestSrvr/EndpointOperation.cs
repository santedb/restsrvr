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
using RestSrvr.Description;

namespace RestSrvr
{
    /// <summary>
    /// Represents a linkage between an endpoint and an operation
    /// </summary>
    public sealed class EndpointOperation
    {
        // The desccription of the operation
        private OperationDescription m_description;
        private OperationDispatcher m_dispatcher;
        private ServiceEndpoint m_endpoint;

        /// <summary>
        /// Gets the endpoint
        /// </summary>
        internal ServiceEndpoint Endpoint => this.m_endpoint;

        /// <summary>
        /// Gets the dispatcher
        /// </summary>
        internal OperationDispatcher Dispatcher => this.m_dispatcher;

        /// <summary>
        /// Gets the description of this operation
        /// </summary>
        public OperationDescription Description => this.m_description;

        /// <summary>
        /// Creates a new instance of the operation with the specified description
        /// </summary>
        /// <param name="operationDescription"></param>
        public EndpointOperation(ServiceEndpoint endpoint, OperationDescription operationDescription)
        {
            this.m_description = operationDescription;
            this.m_endpoint = endpoint;
            this.m_dispatcher = new OperationDispatcher(this);
        }
    }
}