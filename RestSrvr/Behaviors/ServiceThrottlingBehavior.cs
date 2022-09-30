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
using RestSrvr.Exceptions;
using RestSrvr.Message;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;

namespace RestSrvr.Behaviors
{
    /// <summary>
    /// Represents a binding behavior that throttles requests
    /// </summary>
    [DisplayName("Service Throttling")]
    [ExcludeFromCodeCoverage]
    public class ServiceThrottlingBehavior : IServiceBehavior, IMessageInspector
    {
        // Current load
        private int m_currentLoad = 0;

        // Max concurrency
        private int m_maxConcurrency = Environment.ProcessorCount * 16;

        /// <summary>
        /// Service throttling behavior
        /// </summary>
        public ServiceThrottlingBehavior()
        {
        }

        /// <summary>
        /// Service throttling behavior with configuration
        /// </summary>
        /// <remarks>Configuration object
        /// <code lang="xml">
        /// <![CDATA[
        /// <element maxConcurrency="4" />
        /// ]]>
        /// </code> </remarks>
        public ServiceThrottlingBehavior(XElement configuration)
        {
            this.m_maxConcurrency = Int32.Parse(configuration.Attributes().FirstOrDefault(e => e.Name.LocalName == "maxConcurrency")?.Value ?? Environment.ProcessorCount.ToString());
        }

        /// <summary>
        /// After receiving a request increment the max
        /// </summary>
        public void AfterReceiveRequest(RestRequestMessage request)
        {
            Interlocked.Increment(ref this.m_currentLoad);
            if (this.m_currentLoad > this.m_maxConcurrency)
            {
                RestOperationContext.Current.OutgoingResponse.Headers.Add("Retry-After", "1200");
                throw new FaultException((HttpStatusCode)429, "Too Many Requests");
            }
        }

        /// <summary>
        /// Apply the service behavior
        /// </summary>
        public void ApplyServiceBehavior(RestService service, ServiceDispatcher dispatcher)
        {
            foreach (var ep in dispatcher.Service.Endpoints)
            {
                ep.Dispatcher.MessageInspectors.Add(this);
            }
        }

        /// <summary>
        /// Decrement the current load
        /// </summary>
        public void BeforeSendResponse(RestResponseMessage response)
        {
            Interlocked.Decrement(ref this.m_currentLoad);
        }
    }
}