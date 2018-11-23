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
using RestSrvr.Exceptions;
using RestSrvr.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RestSrvr
{
    /// <summary>
    /// The service dispatcher is responsible for linking the HTTP listener to the
    /// RestService
    /// </summary>
    public sealed class EndpointDispatcher
    {

        // Trace source
        private TraceSource m_traceSource = new TraceSource(TraceSources.DispatchTraceSourceName);

        // The rest service that is attached to this dispatcher
        private ServiceEndpoint m_serviceEndpoint;
        private Regex m_endpointRegex;
        private List<IMessageInspector> m_messageInspector = new List<IMessageInspector>();

        /// <summary>
        /// Gets the message inspectors
        /// </summary>
        public List<IMessageInspector> MessageInspectors => this.m_messageInspector;

        /// <summary>
        /// Creates a new endpoint dispatcher for the specified endpoint
        /// </summary>
        public EndpointDispatcher(ServiceEndpoint serviceEndpoint)
        {
            this.m_serviceEndpoint = serviceEndpoint;
            this.m_endpointRegex = new Regex($"^({serviceEndpoint.Description.RawUrl.Replace("://+", "://.+?")})/?.*");
        }

        /// <summary>
        /// Determines if the dispatcher can dispatch the supplied request message
        /// </summary>
        internal bool CanDispatch(RestRequestMessage requestMessage)
        {
            this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "EndpointDispatcher.CanDispatch -> {0} (EPRx: {1})", requestMessage.Url, this.m_endpointRegex);

            // Match the path
            if(this.m_endpointRegex.IsMatch(requestMessage.Url.ToString())) { 
                requestMessage.OperationPath = this.GetOperationPath(requestMessage.Url);
                return true;
            }
            else return false;
        }


        /// <summary>
        /// Dispatch the HttpRequest message to the appropriate service
        /// </summary>
        internal bool Dispatch(ServiceDispatcher serviceDispatcher, RestRequestMessage requestMessage, RestResponseMessage responseMessage)
        {

            // Allow message inspectors to inspect the message before next stage
            try
            {

                this.m_traceSource.TraceEvent(TraceEventType.Verbose, 0, "Begin endpoint dispatch of {0} {1} > {2}", requestMessage.Method, requestMessage.Url, this.m_serviceEndpoint.Description.Contract);

                foreach (var mfi in this.m_messageInspector)
                    mfi.AfterReceiveRequest(requestMessage);
                
                var ops = this.m_serviceEndpoint.Operations.Where(o => o.Dispatcher.CanDispatch(requestMessage));
                if (ops.Count() == 0)
                    throw new FaultException(404, $"Resource not Found - {requestMessage.Url.AbsolutePath}");
                var op = ops.FirstOrDefault(o => requestMessage.Method.ToLowerInvariant() == o.Description.Method.ToLowerInvariant());
                if (op == null)
                    throw new FaultException(405, "Method not permitted");

                RestOperationContext.Current.EndpointOperation = op;
                op.Dispatcher.Dispatch(serviceDispatcher, requestMessage, responseMessage);

                // Allow message inspectors to inspect before sending response
                foreach (var mfi in this.m_messageInspector)
                    mfi.BeforeSendResponse(responseMessage);

                return true;
            }
            catch(Exception e)
            {
                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                return serviceDispatcher.HandleFault(e, responseMessage);
            }
        }
        
        /// <summary>
        /// Gets the operation path (drops the base URL)
        /// </summary>
        internal String GetOperationPath(Uri requestUrl)
        {
            var matches = this.m_endpointRegex.Match(requestUrl.ToString());
            if (matches.Success)
                return requestUrl.ToString().Substring(matches.Groups[1].Value.Length).Split('?')[0];
            else
                throw new InvalidOperationException("Cannot match this path");
        }
    }
}
