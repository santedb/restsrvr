/*
 * Copyright 2015-2019 Mohawk College of Applied Arts and Technology
 * Copyright 2019-2019 SanteSuite Contributors (See NOTICE)
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
 * User: justi
 * Date: 2019-1-12
 */
using RestSrvr.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;

namespace RestSrvr.Description
{
    /// <summary>
    /// Represents a contract description
    /// </summary>
    public sealed class ContractDescription
    {
        // Trace source
        private Tracer m_traceSource = new Tracer(TraceSources.DescriptionTraceSourceName);

        /// <summary>
        /// Operations on the contract
        /// </summary>
        private List<OperationDescription> m_operations = new List<OperationDescription>();

        /// <summary>
        /// Gets the operations
        /// </summary>
        public IEnumerable<OperationDescription> Operations => this.m_operations.AsReadOnly();

        /// <summary>
        /// Gets the name of the contract
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Gets the type of the contract
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Create a new contract description from the specified contract
        /// </summary>
        public ContractDescription(Type contractType)
        {
            this.m_traceSource.TraceEvent(EventLevel.Verbose, "Enter ContractDescription CTOR ({0})", contractType);

            if (!contractType.IsInterface)
                throw new InvalidOperationException("Contract type must be an interface");

            // Get the name of the contract
            this.Type = contractType;
            this.Name = contractType.GetCustomAttribute<ServiceContractAttribute>()?.Name ?? contractType.FullName;
            this.m_operations = contractType.GetRuntimeMethods().Where(m => m.GetCustomAttribute<RestInvokeAttribute>() != null).Select(m => new OperationDescription(this, m)).ToList();
        }
    }
}