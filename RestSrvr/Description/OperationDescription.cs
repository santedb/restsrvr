/*
 * Copyright (C) 2021 - 2022, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2022-5-30
 */
using RestSrvr.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace RestSrvr.Description
{
    /// <summary>
    /// Represents an operation description
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class OperationDescription
    {

        // Trace source
        private TraceSource m_traceSource = new TraceSource(TraceSources.DescriptionTraceSourceName);
        private List<IOperationBehavior> m_operationBehaviors = new List<IOperationBehavior>();

        /// <summary>
        /// Operation description
        /// </summary>
        public OperationDescription(ContractDescription contract, MethodInfo operationMethod, RestInvokeAttribute restAttribute)
        {
            this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Enter OperationDescription CTOR ({0})", operationMethod);
            this.InvokeMethod = operationMethod;
            this.Method = restAttribute.Method;
            this.UriTemplate = restAttribute.UriTemplate;
            this.Contract = contract;
        }

        /// <summary>
        /// Add an operation behavior to the description
        /// </summary>
        public void AddOperationBehavior(IOperationBehavior behavior)
        {
            this.m_operationBehaviors.Add(behavior);
        }

        /// <summary>
        /// Gets the operation behaviors on this behavior
        /// </summary>
        public IEnumerable<IOperationBehavior> Behaviors => this.m_operationBehaviors.AsReadOnly();

        /// <summary>
        /// Invoke the specified method
        /// </summary>
        public MethodInfo InvokeMethod { get; private set; }

        /// <summary>
        /// Gets the method that triggers this operation to be fired
        /// </summary>
        public String Method { get; private set; }

        /// <summary>
        /// The template that should be used to invoke the method
        /// </summary>
        public String UriTemplate { get; private set; }

        /// <summary>
        /// Gets the contract object
        /// </summary>
        public ContractDescription Contract { get; private set; }
    }
}
