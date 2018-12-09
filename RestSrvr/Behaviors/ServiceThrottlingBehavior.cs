﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using RestSrvr.Exceptions;
using RestSrvr.Message;

namespace RestSrvr.Behaviors
{
    /// <summary>
    /// Represents a binding behavior that throttles requests
    /// </summary>
    public class ServiceThrottlingBehavior : IServiceBehavior, IMessageInspector
    {

        // Current load
        private int m_currentLoad = 0;

        // Max concurrency
        private int m_maxConcurrency = Environment.ProcessorCount * 2;

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
            if (this.m_currentLoad > this.m_maxConcurrency) {
                RestOperationContext.Current.OutgoingResponse.Headers.Add("Retry-After", "1200");
                throw new FaultException(429, "Too Many Requests");
            }
        }

        /// <summary>
        /// Apply the service behavior
        /// </summary>
        public void ApplyServiceBehavior(RestService service, ServiceDispatcher dispatcher)
        {
            foreach (var ep in dispatcher.Service.Endpoints)
                ep.Dispatcher.MessageInspectors.Add(this);
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
