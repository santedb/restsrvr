/*
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2019-11-27
 */
using RestSrvr.Description;
using RestSrvr.Message;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace RestSrvr.Bindings
{
    /// <summary>
    /// Represents the binding (HTTP server itself) that actually handles the 
    /// </summary>
    public class RestHttpBinding : IEndpointBinding
    {

        // Trace source
        private TraceSource m_traceSource = new TraceSource(TraceSources.HttpTraceSourceName);

        // The thread which accepts connections
        private Thread m_acceptThread;

        // The HTTP listener that this binding is using
        private HttpListener m_httpListener;

        // The service context
        private ServiceDispatcher m_serviceDispatcher;


        /// <summary>
        /// Attach the specified endpoint to this REST binding
        /// </summary>
        public void AttachEndpoint(ServiceDispatcher serviceDispatcher, ServiceEndpoint endpoint)
        {
            if (this.m_httpListener != null)
                throw new InvalidOperationException("Cannot attach endpoint to running listener");

            this.m_traceSource.TraceEvent(TraceEventType.Information, 0,  "Attaching HTTP listener to endpoint: {0}", endpoint.Description.ListenUri);
            this.m_httpListener = new HttpListener();
            this.m_httpListener.Prefixes.Add(endpoint.Description.RawUrl);
            this.m_serviceDispatcher = serviceDispatcher;
            // Instantiate the 
            this.m_acceptThread = new Thread(() =>
            {
                while (this.m_httpListener != null)
                {
                    try
                    {
                        var accept = this.m_httpListener.GetContext();

                        // Queue work item to run the processing
                        RestServerThreadPool.Current.QueueUserWorkItem((o) =>
                        {
                            var context = accept as HttpListenerContext;
                            try
                            {
                                RestOperationContext.Current = new RestOperationContext(context);
                                var requestMessage = new RestRequestMessage(context.Request);
                                using (var responseMessage = new RestResponseMessage(context.Response))
                                {
                                    this.m_serviceDispatcher.Dispatch(requestMessage, responseMessage);
                                    if(requestMessage.Method.ToLowerInvariant() != "head")
                                        responseMessage.FlushResponseStream();
                                }
                            }
                            catch(Exception e)
                            {
                                this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                            }
                            finally
                            {
                                RestOperationContext.Current.Dispose();
                            }
                        }, accept);

                    }
                    catch (Exception e)
                    {
                        this.m_traceSource.TraceEvent(TraceEventType.Error, e.HResult, e.ToString());
                        this.m_traceSource.TraceEvent(TraceEventType.Critical, e.HResult, "Shutting down listener");
                        this.m_httpListener = null;
                    }
                }
            })
            {
                IsBackground = true,
                Name = $"HttpBinding-{endpoint.Description.ListenUri}"
            };

            this.m_httpListener.Start();
            this.m_acceptThread.Start();

        }

        /// <summary>
        /// Stop the listener
        /// </summary>
        public void DetachEndpoint(ServiceEndpoint endpoint)
        {
            this.m_traceSource?.TraceInformation("Stopping RestHttpBinding services...");
            this.m_httpListener?.Stop();
            this.m_httpListener?.Close();
            this.m_httpListener = null;
        }
    }
}
