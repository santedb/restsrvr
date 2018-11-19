using RestSrvr.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        public void AttachEndpoint(RestService serviceContext, ServiceEndpoint endpoint)
        {
            if (this.m_httpListener != null)
                throw new InvalidOperationException("Cannot attach endpoint to running listener");

            this.m_traceSource.TraceEvent(TraceEventType.Information, 0, "Attaching HTTP listener to endpoint: {0}", endpoint.Description.ListenUri);
            this.m_httpListener = new HttpListener();

            this.m_httpListener.Prefixes.Add(endpoint.Description.ListenUri);


            // TODO: Apply Behaviors
            this.m_serviceDispatcher = new ServiceDispatcher(serviceContext);

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
                                    context.Response.ContentLength64 = responseMessage.Body.Length;

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
                                context.Response.Close();
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
    }
}
